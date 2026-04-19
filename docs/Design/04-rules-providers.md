# Rule Providers — Design

> Describes how `PayrollRuleFactory`, `IPayrollRuleProvider`, and the four built-in providers load pay configuration from the database and deliver pre-configured rules to the engine. Read this before adding a new rule type or a new configuration source.

---

## The Problem This Solves

`PayCalculationEngine` is pure — it must not perform I/O. But rules need data: an employee's base salary, a company's tax rate, a country's minimum wage. Without some mechanism to load this data, rules would have to query the database themselves, which would break the IDesign layering constraint.

The provider pattern solves this by placing all data-loading in the **Manager tier**, where repository calls are permitted. Providers load values from repositories, construct rules with those values injected, and hand a ready-to-run rule list to the engine.

---

## Component Overview

```
PayrollManager / EmployeeManager
        │
        ▼
IPayrollRuleFactory.GetRulesAsync(company, employee)
        │
        ├─── CoreRulesProvider.GetRulesAsync(context)      → [BaseSalaryRule(salary)]
        ├─── CompanyRulesProvider.GetRulesAsync(context)   → [FlatTaxRule(taxRate)]
        ├─── CountryRulesProvider.GetRulesAsync(context)   → [MinimumWageRule(minWage)]
        └─── EmployeeRulesProvider.GetRulesAsync(context)  → [... employee-specific rules]
        │
        ▼
IReadOnlyList<IPayrollRule>   (all values pre-loaded)
        │
        ▼
PayCalculationEngine.Calculate(rules)
```

---

## IPayrollRuleFactory

```csharp
public interface IPayrollRuleFactory
{
    Task<IReadOnlyList<IPayrollRule>> GetRulesAsync(Company company, Employee employee);
}
```

`PayrollRuleFactory` (the implementation) receives all registered `IPayrollRuleProvider` instances via constructor injection and calls each one in sequence, aggregating the results. Adding a new provider is a two-step operation: implement the interface and register it in `Program.cs`.

---

## IPayrollRuleProvider

```csharp
public interface IPayrollRuleProvider
{
    Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context);
}
```

`PayrollRuleContext` carries the `Company` and `Employee` for the current calculation. Providers use these to decide which rules apply and what parameters to load.

---

## The Four Built-in Providers

### CoreRulesProvider
**Scope:** Rules that apply to every employee universally.  
**Repository dependency:** None — values come directly from the employee record already loaded by the manager.  
**Currently produces:** `BaseSalaryRule(employee.BaseSalary)`

### CompanyRulesProvider
**Scope:** Rules driven by the company's configuration (tax scheme, benefit contributions, etc.).  
**Repository dependency:** `ICompanyPayrollConfigRepository` — looks up `CompanyPayrollConfig` by `Company.Id`.  
**Currently produces:** `FlatTaxRule(config.TaxRate)`  
**Returns empty list** when no config row exists for the company.

### CountryRulesProvider
**Scope:** Statutory rules mandated by the employee's country of residence (minimum wage, etc.).  
**Repository dependency:** `ICountryPayrollConfigRepository` — looks up `CountryPayrollConfig` by `Employee.CountryCode`.  
**Currently produces:** `MinimumWageRule(config.MinimumWage)`  
**Returns empty list** when `Employee.CountryCode` is null or no config row exists for that country.

### EmployeeRulesProvider
**Scope:** Rules specific to an individual employee (custom bonuses, personal deductions, etc.).  
**Repository dependency:** (to be wired — stub returns empty list today).  
**Currently produces:** nothing.

---

## Data Flow: Config from DB into a Rule

Using CountryRulesProvider as the example:

```
1. PayrollRuleFactory calls CountryRulesProvider.GetRulesAsync(context)

2. Provider calls ICountryPayrollConfigRepository.GetAsync(context.Employee.CountryCode)
   └─ SQL: SELECT * FROM CountryPayrollConfig WHERE CountryCode = @code

3. Repository returns CountryPayrollConfig { CountryCode = "DK", MinimumWage = 2800 }

4. Provider constructs new MinimumWageRule(2800m) and returns it

5. Engine receives the rule — it has no idea where 2800 came from
   └─ Apply(context, violations): if context.NetPay < 2800 → violation
```

For a Swedish employee (CountryCode = "SE"), the exact same class `MinimumWageRule` is instantiated with Sweden's threshold. No conditional logic anywhere in the engine or the rule.

---

## How to Add a New Country or Company Config

1. Add a row to the relevant config table (`CountryPayrollConfig` or `CompanyPayrollConfig`).
2. No code changes needed — the provider loads by key at runtime.

---

## How to Add a New Rule Type

**Example: a pension contribution deduction (company-configured rate)**

1. **Create the rule** in `src/PayrollCalculator.Engines/Rules/Company/`:

```csharp
public class PensionContributionRule(decimal rate) : IPayrollRule
{
    public PayrollRuleEffect Effect => PayrollRuleEffect.Deduction;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        var amount = Math.Round(context.GrossPay * rate, 2);
        context.TotalDeductions += amount;
        context.LineItems.Add(new PayslipLineItem
        {
            RuleName    = nameof(PensionContributionRule),
            Description = $"Pension contribution ({rate:P0})",
            Amount      = amount,
            Kind        = PayslipLineItemKind.Deduction
        });
    }
}
```

2. **Add a column** (or new table) to store the rate: e.g. `PensionRate` on `CompanyPayrollConfig`.

3. **Update the config model** in `src/PayrollCalculator.Domain/Models/CompanyPayrollConfig.cs`:

```csharp
public record CompanyPayrollConfig
{
    public int     CompanyId    { get; init; }
    public decimal TaxRate      { get; init; }
    public decimal PensionRate  { get; init; }  // ← new
}
```

4. **Update the provider** to emit the rule when the rate is non-zero:

```csharp
// CompanyRulesProvider.GetRulesAsync
var rules = new List<IPayrollRule> { new FlatTaxRule(config.TaxRate) };

if (config.PensionRate > 0)
    rules.Add(new PensionContributionRule(config.PensionRate));

return rules;
```

5. **Update the repository** to read the new column from SQL.

---

## How to Add a New Provider

**Example: a provider for location-based allowances**

1. **Create the provider** in `src/PayrollCalculator.Managers/RuleProviders/`:

```csharp
public sealed class LocationRulesProvider(
    ILocationAllowanceRepository _repo) : IPayrollRuleProvider
{
    public async Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
    {
        if (context.Employee.LocationCode is null)
            return [];

        var config = await _repo.GetAsync(context.Employee.LocationCode);
        if (config is null)
            return [];

        return [new LocationAllowanceRule(config.AllowanceAmount)];
    }
}
```

2. **Register it** in `Program.cs` alongside the existing providers:

```csharp
builder.Services.AddScoped<IPayrollRuleProvider, LocationRulesProvider>();
```

`PayrollRuleFactory` picks it up automatically via the `IEnumerable<IPayrollRuleProvider>` collection in its constructor — no factory changes required.

---

## Rules for Provider Authors

- **Providers must not modify the employee or company records.** They are read-only callers.
- **Return an empty enumerable, not null**, when no rules apply (null guard at the factory level is not a substitute for proper provider hygiene).
- **Inject only the repositories the provider actually needs.** Avoid accepting `IPayrollRepository` or other wide interfaces when a targeted config repo suffices.
- **Rules must not retain a reference to `PayrollRuleContext` or any entity.** Inject only the scalar values the rule needs (rate, amount, threshold). This keeps rules testable without any database or entity setup.
