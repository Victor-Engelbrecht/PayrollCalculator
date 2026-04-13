# System Architecture — IDesign Decomposition

> Shows all components grouped by IDesign tier and the only call directions that are permitted. Use this as the canonical reference during code review — any dependency that does not appear here is a design violation.

## IDesign Call Rules

| Layer | May Call | May NOT Call |
|---|---|---|
| **Client** | Managers only | Engines, Repositories, Adapters |
| **Manager** | Engines, Repositories, Adapters | Other Managers |
| **Engine** | Nothing (pure logic, value-in/value-out) | Repositories, Adapters, Managers |
| **Repository** | External DB only | Engines, Adapters, other Repositories |
| **Adapter** | External services only | Engines, Repositories, other Adapters |

## Forbidden Calls Checklist

Use during code review to verify no shortcut dependencies were introduced:

- [ ] No Controller/API class injects or calls an Engine, Repository, or Adapter directly
- [ ] No Manager calls another Manager
- [ ] No Engine has a dependency on a Repository, Adapter, or Manager
- [ ] No Repository calls another Repository, an Engine, or an Adapter
- [ ] No Adapter calls a Repository, Engine, or another Adapter

## Diagram

```mermaid
graph TD
    HTTPClient([HTTP Client / Swagger UI])

    subgraph tier_client["Client Tier — PayrollCalculator.Client"]
        API[Web API\nControllers]
    end

    subgraph tier_managers["Manager Tier — PayrollCalculator.Managers"]
        CompanyMgr[CompanyManager]
        EmployeeMgr[EmployeeManager]
        PayrollMgr[PayrollManager]
    end

    subgraph tier_engines["Engine Tier — PayrollCalculator.Engines"]
        PayCalcEng[PayCalculationEngine]
        AuditEng[AuditEngine]
    end

    subgraph tier_accessors["Accessor Tier"]
        subgraph repos["Repositories — PayrollCalculator.Repositories"]
            CompanyRepo[CompanyRepository]
            EmployeeRepo[EmployeeRepository]
            PayrollRepo[PayrollRepository]
            PayslipRepo[PayslipRepository]
            AddDeductRepo[AdditionDeductionRepository]
        end
        subgraph adapters["Adapters — PayrollCalculator.Adapters"]
            PaymentAdp[PaymentProviderAdapter]
            EmailAdp[EmailAdapter]
        end
    end

    subgraph tier_external["External Systems"]
        SqlServer[(SQL Server)]
        PaymentAPI([Payment Provider API])
        EmailSvc([Email Service])
    end

    subgraph tier_shared["Shared — referenced by all tiers"]
        Abstractions[PayrollCalculator.Abstractions\nDomain Models]
        Utilities[PayrollCalculator.Utilities]
    end

    %% External → Client
    HTTPClient --> API

    %% Client → Managers
    API --> CompanyMgr
    API --> EmployeeMgr
    API --> PayrollMgr

    %% Managers → Engines
    CompanyMgr --> PayCalcEng
    PayrollMgr --> PayCalcEng
    PayrollMgr --> AuditEng

    %% Managers → Repositories
    CompanyMgr --> CompanyRepo
    EmployeeMgr --> EmployeeRepo
    EmployeeMgr --> AddDeductRepo
    PayrollMgr --> EmployeeRepo
    PayrollMgr --> PayrollRepo
    PayrollMgr --> PayslipRepo
    PayrollMgr --> AddDeductRepo

    %% Managers → Adapters
    PayrollMgr --> PaymentAdp
    PayrollMgr --> EmailAdp

    %% Repositories → External DB
    CompanyRepo --> SqlServer
    EmployeeRepo --> SqlServer
    PayrollRepo --> SqlServer
    PayslipRepo --> SqlServer
    AddDeductRepo --> SqlServer

    %% Adapters → External Services
    PaymentAdp --> PaymentAPI
    EmailAdp --> EmailSvc

    %% Styling
    classDef clientStyle fill:#4A90D9,color:#fff,stroke:#2c6fad
    classDef managerStyle fill:#7B68EE,color:#fff,stroke:#5a4db5
    classDef engineStyle fill:#F5A623,color:#fff,stroke:#c07d0a
    classDef repoStyle fill:#7ED321,color:#fff,stroke:#5a9a12
    classDef adapterStyle fill:#50C878,color:#fff,stroke:#2e9e52
    classDef externalStyle fill:#9B9B9B,color:#fff,stroke:#666
    classDef sharedStyle fill:#F8F8F8,color:#333,stroke:#ccc,stroke-dasharray:5 5

    class API clientStyle
    class CompanyMgr,EmployeeMgr,PayrollMgr managerStyle
    class PayCalcEng,AuditEng engineStyle
    class CompanyRepo,EmployeeRepo,PayrollRepo,PayslipRepo,AddDeductRepo repoStyle
    class PaymentAdp,EmailAdp adapterStyle
    class SqlServer,PaymentAPI,EmailSvc,HTTPClient externalStyle
    class Abstractions,Utilities sharedStyle
```

## Key Design Decisions

1. **PayrollManager is the only orchestrator for payroll runs.** It calls both Adapters (payment, email) — this is deliberate. No separate "PaymentManager" or "EmailManager" exists because IDesign forbids Manager-to-Manager calls, and these are not independent use cases.
2. **Engines receive and return value objects only.** `PayCalculationEngine` takes salary + additions + deductions and returns a net amount — no database access, no side effects.
3. **`AuditEngine` is logic, not persistence.** It computes audit/summary data. `PayrollManager` is responsible for persisting the result via a Repository.
4. **Adapters are treated as Accessors.** They sit in the same tier as Repositories and are called only by Managers.
