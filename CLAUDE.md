# CLAUDE.md — PayrollCalculator

This file provides guidance for AI assistants working in this repository.

---

## Project Overview

PayrollCalculator is an ASP.NET Core 8 Web API that manages companies, employees, payroll runs, and payslip generation. It integrates with external payment and email providers.

**Status**: Early development. Domain models and API controllers are scaffolded. The `PayCalculationEngine` (with `FlatTaxRule` and `MinimumWageRule`) is implemented. All repositories, adapters, and manager orchestration are stubs throwing `NotImplementedException`.

---

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 8 / C# 11+ |
| API Framework | ASP.NET Core Web API |
| ORM / Data Access | Dapper 2.1.72 + Microsoft.Data.SqlClient 7.0.0 |
| Database | Microsoft SQL Server (localhost, `PayrollCalculator` database) |
| Testing | NUnit 3.14.0 + NSubstitute 5.3.0 + coverlet |
| DI | Microsoft.Extensions.DependencyInjection |
| API Docs | Swashbuckle / Swagger |

---

## Repository Layout

```
/
├── src/
│   ├── PayrollCalculator.Client/          # ASP.NET Core entry point
│   │   ├── Controllers/                   # HTTP endpoints (thin — delegate to Managers)
│   │   ├── Requests/                      # Inbound DTO records
│   │   ├── Responses/                     # Outbound DTO records
│   │   ├── Workers/                       # Background services
│   │   ├── appsettings.json               # Connection strings + config
│   │   └── Properties/launchSettings.json # HTTP :5102 / HTTPS :7099
│   ├── PayrollCalculator.Managers/        # Orchestration layer
│   │   └── Contracts/                     # ICompanyManager, IEmployeeManager, IPayrollManager
│   ├── PayrollCalculator.Engines/         # Pure calculation logic
│   │   ├── Contracts/                     # IPayCalculationEngine, IPayrollRule
│   │   └── Rules/                         # FlatTaxRule (20%), MinimumWageRule (net ≥ 1500)
│   ├── PayrollCalculator.Repositories/    # Dapper SQL data access (stubs)
│   │   └── Contracts/                     # ICompanyRepository, IEmployeeRepository, IPayrollRepository
│   ├── PayrollCalculator.Adapters/        # External service integrations (stubs)
│   │   └── Contracts/                     # IPaymentAdapter, IEmailAdapter
│   ├── PayrollCalculator.Domain/          # Domain models and adapter DTOs
│   │   └── Models/                        # Plain C# records: Company, Employee, Payroll, PayslipDetail, etc.
│   └── PayrollCalculator.Utilities/       # Shared helpers
├── tests/
│   ├── unit/PayrollCalculator.UnitTests/  # NUnit unit tests (currently placeholder)
│   └── integration/PayrollCalculator.IntegrationTests/ # NUnit integration tests (placeholder)
├── docs/
│   ├── Design/01-architecture.md          # IDesign layered architecture overview
│   └── requirements.md                    # Product and technical requirements
└── PayrollCalculator.sln
```

---

## Architecture: IDesign Layered Architecture

The project strictly follows the **IDesign** layered approach. Violating the call rules below is a bug, not a style issue.

### Layers and Allowed Calls

```
Client (Controllers)
    ↓  calls
Managers (Orchestrators)
    ↓  calls
Engines  |  Repositories  |  Adapters
```

| Layer | Role | May call |
|---|---|---|
| **Client** | HTTP in/out, request validation | Managers only |
| **Managers** | Orchestrate a workflow | Engines, Repositories, Adapters |
| **Engines** | Pure business logic | Nothing (value-in / value-out) |
| **Repositories** | SQL data access via Dapper | Nothing above |
| **Adapters** | External services (payment, email) | Nothing above |

### Forbidden Calls (enforce in code review)
- Controllers must NOT call Repositories, Engines, or Adapters directly.
- Managers must NOT call other Managers.
- Engines must NOT perform I/O of any kind.
- Repositories and Adapters must NOT call Managers or Engines.

---

## Domain Models

All models are C# `record` types (immutable value objects) in `PayrollCalculator.Domain/Models/`.

| Model | Key Fields |
|---|---|
| `Company` | Id, Name, RegistrationNumber, ContactEmail?, CreatedAt, UpdatedAt |
| `Employee` | Id, CompanyId, FirstName, LastName, Email, BaseSalary, BankAccountNumber?, CreatedAt, UpdatedAt |
| `Payroll` | Id, CompanyId, PeriodStart, PeriodEnd, RunAt, Status, CreatedAt, UpdatedAt |
| `PayslipDetail` | Id, PayrollId, EmployeeId, BaseSalary, TotalAdditions, TotalDeductions, NetAmount, PaymentReference?, CreatedAt, UpdatedAt |
| `Addition` | Id, EmployeeId, PayrollId, Description, Amount, CreatedAt, UpdatedAt |
| `Deduction` | Id, EmployeeId, PayrollId, Description, Amount, CreatedAt, UpdatedAt |
| `PayCalculationContext` | Employee, Additions[], Deductions[], GrossPay, TotalDeductions, NetPay (internal) |
| `PayCalculationResult` | NetAmount, TotalAdditions, TotalDeductions, Violations[] |
| `PayrollSummary` | PayrollId, EmployeeCount, TotalNetPaid, Violations[], CompletedAt |
| `RuleViolation` | RuleName, Message |

---

## API Endpoints

Base URL in development: `http://localhost:5102`

### Companies
```
GET    /companies                               List all companies
GET    /companies/{id}                          Get company by ID
POST   /companies                               Create company
PUT    /companies/{id}                          Update company
DELETE /companies/{id}                          Delete company
```

### Employees & Pay Items
```
GET    /employees/{id}                          Get employee by ID
GET    /companies/{companyId}/employees         List employees for a company
POST   /employees                               Create employee
PUT    /employees/{id}                          Update employee
DELETE /employees/{id}                          Delete employee

GET    /employees/{id}/additions                List additions
POST   /employees/{id}/additions                Add addition
DELETE /employees/{id}/additions/{additionId}   Remove addition

GET    /employees/{id}/deductions               List deductions
POST   /employees/{id}/deductions               Add deduction
DELETE /employees/{id}/deductions/{deductionId} Remove deduction
```

### Payroll
```
POST   /payroll/run                             Run payroll (RunPayrollRequest)
GET    /payroll/{id}                            Get payroll by ID
GET    /payroll/{id}/payslips                   Get payslips for a run
```

---

## Business Rules (Engines)

All rules implement `IPayrollRule` and are applied by `PayCalculationEngine.Calculate()`.

| Rule | Logic |
|---|---|
| `FlatTaxRule` | Deducts 20% tax from gross pay (BaseSalary + Additions) |
| `MinimumWageRule` | Adds a `RuleViolation` if net pay < 1500; does not block processing |

**Net pay formula**: `(BaseSalary + TotalAdditions) - TotalDeductions - Tax`

To add a new rule: implement `IPayrollRule`, register it in DI, and it will be picked up automatically by the engine.

---

## Development Workflow

### Build
```bash
dotnet build PayrollCalculator.sln
```

### Run API
```bash
dotnet run --project src/PayrollCalculator.Client
# Swagger UI: http://localhost:5102/swagger
```

### Run Tests
```bash
dotnet test PayrollCalculator.sln
```

### Database
The app connects to a local SQL Server instance:
```
Server=localhost;Database=PayrollCalculator;Trusted_Connection=True;TrustServerCertificate=True;
```
No migration tooling is configured yet. Schema DDL must be applied manually or via a migration tool added later.

---

## Naming Conventions

| Artifact | Pattern | Example |
|---|---|---|
| Controller | `*Controller` | `CompaniesController` |
| Manager | `*Manager` / `I*Manager` | `PayrollManager` / `IPayrollManager` |
| Repository | `*Repository` / `I*Repository` | `EmployeeRepository` / `IEmployeeRepository` |
| Adapter | `*Adapter` / `I*Adapter` | `PaymentProviderAdapter` / `IPaymentAdapter` |
| Engine | `*Engine` / `I*Engine` | `PayCalculationEngine` / `IPayCalculationEngine` |
| Rule | `*Rule` (implements `IPayrollRule`) | `FlatTaxRule` |
| Request DTO | `*Request` | `RunPayrollRequest` |
| Response DTO | `*Response` | `PayrollSummaryResponse` |
| Domain model | PascalCase noun record | `Employee`, `PayslipDetail` |

---

## Coding Conventions

- **Nullable reference types** are enabled (`<Nullable>enable</Nullable>`). Use `?` for genuinely optional values; never suppress warnings with `!` without justification.
- **Implicit usings** are enabled; do not add redundant `using` statements for `System`, `System.Collections.Generic`, etc.
- **Records** are preferred for value objects and DTOs.
- **Async throughout**: all I/O methods return `Task<T>`. Method names end with `Async`.
- **Constructor injection** for all dependencies; no service locator or static access.
- Interfaces live in a `Contracts/` subfolder within each project.
- Do not add comments to self-explanatory code. Comments are reserved for non-obvious business logic (e.g., why a rule threshold is 1500).

---

## Testing Guidelines

- **Unit tests** go in `tests/unit/PayrollCalculator.UnitTests/`.
- **Integration tests** go in `tests/integration/PayrollCalculator.IntegrationTests/`.
- Use **NSubstitute** (`Substitute.For<T>()`) for mocking interfaces.
- Tests follow the Arrange / Act / Assert pattern.
- Engine tests should be pure (no mocks needed — engines have no I/O dependencies).
- Manager tests mock all Engines, Repositories, and Adapters.

---

## Current Implementation Status

| Component | Status |
|---|---|
| Domain models | Complete |
| API controllers (scaffold) | Complete |
| Request / Response DTOs | Complete |
| `PayCalculationEngine` | Complete |
| `FlatTaxRule`, `MinimumWageRule` | Complete |
| Manager interfaces | Defined |
| Repository interfaces | Defined |
| Adapter interfaces | Defined |
| Manager implementations | Stub (`NotImplementedException`) |
| Repository implementations | Stub (`NotImplementedException`) |
| Adapter implementations | Stub (`NotImplementedException`) |
| Unit tests | Placeholder only |
| Integration tests | Placeholder only |
| Database schema / migrations | Not started |
| CI/CD | Not configured |

---

## Out of Scope (per requirements)

- Multi-jurisdiction / multi-currency tax calculations
- Approval workflows for payroll runs
- Multiple payment provider integrations (single provider only)
