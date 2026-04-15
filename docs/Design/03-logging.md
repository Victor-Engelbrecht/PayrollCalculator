# Logging — Design

> Describes how logs are emitted, what they contain, and the rules every layer must follow. Use this as the reference when adding new managers, endpoints, or background workers. Also read this before "just adding a quick `LogInformation`" — that almost certainly violates the design.

---

## Philosophy: One Wide Event Per Request

This project rejects the traditional "scatter `log.Info(...)` statements through the code and grep later" approach. Grepping is not a querying strategy — it falls apart the moment you have more than one service, more than one customer, or a non-trivial distributed bug.

Instead, each HTTP request produces **exactly one rich JSON log event** — a *canonical log line* / *wide event* — that carries every field you could want to filter or group by afterwards. That event is built up incrementally as the request flows through the stack, then emitted once at the end.

The philosophy is summarised on [loggingsucks.com](https://loggingsucks.com/). Our implementation is the idiomatic .NET realisation of it: **Serilog + `UseSerilogRequestLogging` + `IDiagnosticContext`**.

### Mental model shift

| Old way | This project |
|---|---|
| Log what your *code* is doing | Log what happened to this *request* |
| 10–20 short log lines per request | 1 wide event per request |
| Plain strings, grep later | Structured JSON, query by field |
| Sprinkle `LogInformation("calling payment provider")` | Set `payments_processed = N` on the request event |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│  HTTP Request                                                   │
│       │                                                         │
│       ▼                                                         │
│  CorrelationIdMiddleware         (reads/generates X-Correlation-Id,
│       │                           echoes in response)           │
│       ▼                                                         │
│  UseSerilogRequestLogging        (opens a DiagnosticContext,    │
│       │                           will emit 1 log at the end)   │
│       ▼                                                         │
│  Controller ──► Manager                                         │
│                     │                                           │
│                     ▼                                           │
│                 IWideEventContext.Set("company_id", 42)         │
│                 IWideEventContext.Set("employee_count", 17)     │
│                     │                                           │
│                     ▼                                           │
│              (repository / engine / adapter work)               │
│       │                                                         │
│       ▼                                                         │
│  UseSerilogRequestLogging emits ONE JSON event containing:      │
│    • automatic request metadata (method, path, status, elapsed) │
│    • correlation & trace IDs                                    │
│    • everything set on the DiagnosticContext                    │
│    • any exception that escaped                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Components

| Component | Location | Role |
|---|---|---|
| `IWideEventContext` | `PayrollCalculator.Utilities/Contracts/` | Thin abstraction Managers depend on. `Set(name, value)` attaches a field to the current request's wide event. |
| `NullWideEventContext` | `PayrollCalculator.Utilities/` | No-op implementation. Used outside HTTP contexts (e.g. background workers, startup, tests). |
| `SerilogWideEventContext` | `PayrollCalculator.Client/Logging/` | Adapter that forwards to Serilog's `IDiagnosticContext`. Registered in DI as `IWideEventContext`. |
| `CorrelationIdMiddleware` | `PayrollCalculator.Client/Middleware/` | Reads incoming `X-Correlation-Id` header (generates a GUID if absent), stores in `HttpContext.Items`, echoes in the response. |
| `UseSerilogRequestLogging` | `Serilog.AspNetCore` package | Opens a request-scoped `DiagnosticContext`, captures method/path/status/elapsed, emits the single log event. |
| `EnrichDiagnosticContext` callback | `Program.cs` | Runs at request end; adds `request_id`, `correlation_id`, `user_agent`, `client_ip`, etc. before the event is emitted. |

### Why the abstraction?

Managers must remain HTTP-agnostic (IDesign rule — see [01-architecture.md](01-architecture.md)). If they took a direct dependency on Serilog's `IDiagnosticContext`, they would be coupled to ASP.NET / HTTP. `IWideEventContext` is a three-line interface in `Utilities` that keeps Managers portable; the Client project owns the Serilog wiring.

---

## Pipeline Order Matters

The middleware pipeline in [Program.cs](../../src/PayrollCalculator.Client/Program.cs) is ordered deliberately:

```csharp
app.UseMiddleware<CorrelationIdMiddleware>();   // 1. stash correlation_id on HttpContext.Items
app.UseSerilogRequestLogging(opts => { ... });  // 2. opens DiagnosticContext, reads Items
app.UseSwagger();                               // 3. app middleware
app.MapControllers();
```

If `UseSerilogRequestLogging` ran first, the enrichment callback would find no `correlation_id` in `HttpContext.Items` and the field would be missing from every event. Do not reorder these without reading this note.

---

## Fields on the Canonical Event

A successful payroll run produces a log event with roughly **30 fields**. This is deliberate high dimensionality — the more fields, the more queries are possible without code changes.

### Automatic (Serilog + enrichers)

| Field | Source | Example |
|---|---|---|
| `@t` | Serilog | `"2026-04-14T21:47:03.1234567Z"` |
| `@l` | Serilog | `"Information"` |
| `@mt` | Serilog | `"HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms"` |
| `RequestMethod` | `UseSerilogRequestLogging` | `"POST"` |
| `RequestPath` | `UseSerilogRequestLogging` | `"/payroll/run"` |
| `StatusCode` | `UseSerilogRequestLogging` | `200` |
| `Elapsed` | `UseSerilogRequestLogging` | `147.3` (ms) |
| `MachineName` | `WithMachineName` enricher | `"pod-f4x9"` |
| `ThreadId` | `WithThreadId` enricher | `14` |
| `EnvironmentName` | `WithEnvironmentName` enricher | `"Development"` |
| `Application` | `Properties` in appsettings | `"PayrollCalculator"` |
| `SourceContext` | `FromLogContext` enricher | `"Serilog.AspNetCore.RequestLoggingMiddleware"` |
| `TraceId` / `SpanId` | .NET 8 W3C trace context | free, propagates across services |

### Added by `EnrichDiagnosticContext` in `Program.cs`

| Field | Source |
|---|---|
| `request_id` | `HttpContext.TraceIdentifier` |
| `correlation_id` | `HttpContext.Items["CorrelationId"]` (from `CorrelationIdMiddleware`) |
| `user_agent` | `Request.Headers.UserAgent` |
| `client_ip` | `Connection.RemoteIpAddress` |
| `query_string` | `Request.QueryString` |
| `content_length` | `Request.ContentLength` |
| `response_content_type` | `Response.ContentType` |

### Added by Managers (domain fields)

Every manager method begins by setting at least an `operation` tag. Additional fields are set as the method progresses.

**Common:**

| Field | Set by | Notes |
|---|---|---|
| `operation` | every manager method | e.g. `"payroll.run"`, `"company.get"`. Acts as a high-cardinality filter for dashboards. |

**`PayrollManager.RunPayrollAsync`** (the prime case — a multi-step workflow):

| Field | When set | Type |
|---|---|---|
| `company_id` | entry | int |
| `period_start`, `period_end` | entry | DateTime |
| `employee_count` | after employee fetch | int |
| `payroll_id` | after payroll row created | int |
| `payroll_status` | before return | string |
| `payslips_generated` | before return | int |
| `total_gross` | before return | decimal |
| `total_additions` | before return | decimal |
| `total_deductions` | before return | decimal |
| `total_net_paid` | before return | decimal |
| `rule_violations_count` | before return | int |
| `rule_violations` | before return | string[] (distinct rule names) |
| `payments_processed` | before return | int |
| `payment_failures` | before return | int |
| `emails_sent` | before return | int |
| `email_failures` | before return | int |

### On error

`UseSerilogRequestLogging` automatically attaches the unhandled exception to the event and raises the level to `Error`. Managers may add an `error_stage` field *before* rethrowing when the failure point is business-meaningful (e.g. `"payment_dispatch"`, `"email_send"`). They do not catch-and-swallow.

---

## Rules of the Road

### DO

- **Inject `IWideEventContext` into every Manager.** Set `operation` and any relevant IDs at the start of every method.
- **Name fields in `snake_case`.** Easier to query in downstream systems and visually distinct from Serilog's automatic PascalCase fields.
- **Prefer numbers and IDs over strings.** `employee_count: 17` is queryable; `message: "processed 17 employees"` is not.
- **Set aggregate fields at the end.** In loops, accumulate counters (`emailsSent++`) and emit one summary field, not one event per iteration.
- **Think "what would I filter by at 3am?"** Company ID, correlation ID, error stage, feature flag — set them.

### DO NOT

- **Do not inject `ILogger<T>` into Managers, Engines, Repositories, or Adapters.** Adding `logger.LogInformation(...)` calls inside business code defeats the entire design. One event per request, not twenty.
- **Do not emit extra events from middleware.** `UseSerilogRequestLogging` already emits one. Adding another is duplication and noise.
- **Do not put secrets, full request bodies, or PII on the event.** Passwords, tokens, bank account numbers, full names + DOB, etc. have no place in logs. Employee IDs yes; `BankAccountNumber` values no.
- **Do not log inside Engines.** Engines are pure, by design (see [01-architecture.md](01-architecture.md)). They take input and return output. Logging is I/O.
- **Do not rely on `ILogger` scope / `LogContext.PushProperty` for request-scoped data.** That adds properties to *every* log event in scope — fine for background tasks that legitimately log multiple events, wrong for HTTP requests where we want exactly one.

### The one exception

Serilog's own framework logs (startup, shutdown, unhandled errors in hosting) pass through untouched — those are framework events, not request events. They appear as separate JSON lines at process start/stop and are expected.

---

## Configuration

Defined in [appsettings.json](../../src/PayrollCalculator.Client/appsettings.json) under the `Serilog` section. The Development override in [appsettings.Development.json](../../src/PayrollCalculator.Client/appsettings.Development.json) drops `MinimumLevel.Default` from `Information` to `Debug`.

**Output**: **stdout only**, formatted as compact JSON via `Serilog.Formatting.Compact.CompactJsonFormatter`. Chosen because the app is containerised — Docker / Kubernetes / ECS all collect container stdout natively. No file sinks, no network sinks. Ship logs downstream via the platform, not the app.

**Sampling**: not implemented. The wide-event philosophy advocates *tail sampling* (keep all errors + slow requests, sample 1–5 % of fast successes) but that's an ingestion-tier concern — Vector, Honeycomb, Seq, Loki all support it. The app logs everything; the platform decides what to keep.

---

## Sample Output

Request:

```http
POST /payroll/run HTTP/1.1
X-Correlation-Id: checkout-7c3-abc

{ "companyId": 42, "periodStart": "2026-04-01", "periodEnd": "2026-04-30" }
```

Emitted log line (pretty-printed for readability — actual output is a single line):

```json
{
  "@t": "2026-04-14T21:47:03.1234567Z",
  "@l": "Information",
  "@mt": "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
  "RequestMethod": "POST",
  "RequestPath": "/payroll/run",
  "StatusCode": 200,
  "Elapsed": 147.3,
  "request_id": "0HN8A3B4F5C6D:00000001",
  "correlation_id": "checkout-7c3-abc",
  "user_agent": "curl/8.5.0",
  "client_ip": "172.18.0.1",
  "query_string": "",
  "content_length": 72,
  "response_content_type": "application/json; charset=utf-8",
  "operation": "payroll.run",
  "company_id": 42,
  "period_start": "2026-04-01T00:00:00",
  "period_end": "2026-04-30T00:00:00",
  "employee_count": 17,
  "payroll_id": 904,
  "payroll_status": "Completed",
  "payslips_generated": 17,
  "total_gross": 510000.00,
  "total_additions": 12500.00,
  "total_deductions": 102000.00,
  "total_net_paid": 420500.00,
  "rule_violations_count": 2,
  "rule_violations": ["MinimumWageRule"],
  "payments_processed": 15,
  "payment_failures": 0,
  "emails_sent": 17,
  "email_failures": 0,
  "MachineName": "pod-f4x9",
  "ThreadId": 14,
  "EnvironmentName": "Development",
  "Application": "PayrollCalculator",
  "TraceId": "0af7651916cd43dd8448eb211c80319c",
  "SpanId": "b7ad6b7169203331"
}
```

**What you can now query** without any code changes:

- *"Show all payroll runs for company 42 in the last 24 hours."* → `company_id:42 AND operation:"payroll.run"`
- *"Which payroll runs had payment failures?"* → `payment_failures:>0`
- *"Trace this user's complaint — they sent us `X-Correlation-Id: checkout-7c3-abc`."* → `correlation_id:"checkout-7c3-abc"` returns every event touched by that request across the whole system.
- *"Rule violations over the last week, grouped by rule."* → aggregate over `rule_violations`.

---

## Extending to New Surfaces

### New Manager or Controller endpoint

1. Constructor-inject `IWideEventContext` (Managers) — Controllers usually don't need it; enrichment happens below them.
2. At the top of each public method, call `_wideEvent.Set("operation", "<area>.<verb>")` and set any primary IDs.
3. Set aggregate/outcome fields just before returning.

### New Field

Just call `_wideEvent.Set("<snake_case_name>", value)`. No registration step. Fields are schemaless — add freely, but keep the naming consistent with existing fields.

### Background Workers (`PayrollWorker` and friends)

Workers run outside an HTTP request, so there's no active `DiagnosticContext`. Two options:

1. **One event per iteration** (preferred for scheduled / batch jobs): have the worker open a new Serilog `LogContext` scope per iteration, log a single summary event at the end using `Log.Information(...)` with all the same snake_case fields on the message template. Inject `NullWideEventContext` into any Managers called from the worker so they don't crash.
2. **No wide event** for truly internal housekeeping iterations that have no interesting state — just let framework logging handle lifecycle.

The worker is a `NotImplementedException` stub today; revisit when it actually does something.

### Distributed tracing / OpenTelemetry

.NET 8 already surfaces `TraceId` and `SpanId` on every event automatically via `Activity`. If we later add OpenTelemetry for cross-service propagation, Serilog integrates — but OTel is a *transport* concern, not a *what-to-log* concern. The wide-event pattern is orthogonal to it.

---

## Reference

- [loggingsucks.com](https://loggingsucks.com/) — the external reference for the philosophy
- [Serilog.AspNetCore docs](https://github.com/serilog/serilog-aspnetcore) — `UseSerilogRequestLogging` / `IDiagnosticContext` API
- [01-architecture.md](01-architecture.md) — why `IWideEventContext` lives in Utilities, not Client
