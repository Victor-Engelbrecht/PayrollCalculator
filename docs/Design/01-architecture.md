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
        RuleProviders[Rule Providers\nCoreRulesProvider\nCompanyRulesProvider\nCountryRulesProvider\nEmployeeRulesProvider]
    end

    subgraph tier_engines["Engine Tier — PayrollCalculator.Engines"]
        PayCalcEng[PayCalculationEngine]
        NotifEng[PayrollNotificationEngine]
        RuleFactory[PayrollRuleFactory]
    end

    subgraph tier_accessors["Accessor Tier"]
        subgraph repos["Repositories — PayrollCalculator.Repositories"]
            CompanyRepo[CompanyRepository]
            EmployeeRepo[EmployeeRepository]
            PayrollRepo[PayrollRepository]
            CompanyConfigRepo[CompanyPayrollConfigRepository]
            CountryConfigRepo[CountryPayrollConfigRepository]
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
        Domain[PayrollCalculator.Domain\nDomain Models]
        Utilities[PayrollCalculator.Utilities]
    end

    %% External → Client
    HTTPClient --> API

    %% Client → Managers
    API --> CompanyMgr
    API --> EmployeeMgr
    API --> PayrollMgr

    %% Managers → Engines
    EmployeeMgr --> PayCalcEng
    PayrollMgr --> PayCalcEng
    PayrollMgr --> NotifEng
    PayrollMgr --> RuleFactory
    EmployeeMgr --> RuleFactory
    RuleFactory --> RuleProviders

    %% Managers → Repositories
    CompanyMgr --> CompanyRepo
    EmployeeMgr --> EmployeeRepo
    PayrollMgr --> EmployeeRepo
    PayrollMgr --> PayrollRepo
    RuleProviders --> CompanyConfigRepo
    RuleProviders --> CountryConfigRepo

    %% Managers → Adapters
    PayrollMgr --> PaymentAdp
    PayrollMgr --> EmailAdp

    %% Repositories → External DB
    CompanyRepo --> SqlServer
    EmployeeRepo --> SqlServer
    PayrollRepo --> SqlServer
    CompanyConfigRepo --> SqlServer
    CountryConfigRepo --> SqlServer

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
    class CompanyMgr,EmployeeMgr,PayrollMgr,RuleProviders managerStyle
    class PayCalcEng,NotifEng,RuleFactory engineStyle
    class CompanyRepo,EmployeeRepo,PayrollRepo,CompanyConfigRepo,CountryConfigRepo repoStyle
    class PaymentAdp,EmailAdp adapterStyle
    class SqlServer,PaymentAPI,EmailSvc,HTTPClient externalStyle
    class Domain,Utilities sharedStyle
```

## Key Design Decisions

1. **PayrollManager is the only orchestrator for payroll runs.** It calls both Adapters (payment, email) — this is deliberate. No separate "PaymentManager" or "EmailManager" exists because IDesign forbids Manager-to-Manager calls, and these are not independent use cases.

2. **Engines are pure — no entity data crosses the engine boundary.** `PayCalculationEngine.Calculate` receives only pre-configured rule objects; it never sees an `Employee` or `Company`. All entity data is loaded by providers (Manager tier) and baked into rule constructors before the engine is called. See [04-rules-providers.md](04-rules-providers.md).

3. **Rule providers own all data-loading for pay calculation.** `CoreRulesProvider`, `CompanyRulesProvider`, `CountryRulesProvider`, and `EmployeeRulesProvider` call their respective repositories and inject loaded values (salary, tax rate, minimum wage) into rule constructors. The engine never needs to know where those values came from.

4. **Adapters are treated as Accessors.** They sit in the same tier as Repositories and are called only by Managers.

5. **`PayrollRuleFactory` aggregates providers via DI.** All registered `IPayrollRuleProvider` implementations are injected as a collection. Adding a new provider is a two-step operation: implement `IPayrollRuleProvider` and register it in `Program.cs` — no factory code changes.
