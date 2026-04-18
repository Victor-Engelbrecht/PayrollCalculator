# Repository Improvement Tasks

Tracking improvements to [src/PayrollCalculator.Repositories/](../src/PayrollCalculator.Repositories/). Tick items off as they land; leave discussion inline under each item.

## High-Priority

- [ ] **T1 — Transactional boundary for `RunPayrollAsync`**
      Introduce `IUnitOfWork` so payroll writes share one connection/transaction.
      Currently [PayrollManager.cs:45-196](../src/PayrollCalculator.Managers/PayrollManager.cs#L45) performs many writes across separate connections with no atomicity — a mid-run failure leaves orphaned records and a payroll stuck in `Running`.
      Files: `PayrollRepository.cs`, `IPayrollRepository.cs`, `PayrollManager.cs`, `Program.cs` (DI), new `IUnitOfWork` + `UnitOfWork` in repositories project.
      Notes:

- [ ] **T2 — Delete dead `GetAdditionsByPayslipAsync` / `GetDeductionsByPayslipAsync`**
      Zero callers (grep-confirmed). Remove from [PayrollRepository.cs:111-140](../src/PayrollCalculator.Repositories/PayrollRepository.cs#L111) and [IPayrollRepository.cs:18-21](../src/PayrollCalculator.Repositories/Contracts/IPayrollRepository.cs#L18).
      Notes:

- [ ] **T3 — Raise `RecordNotFoundException` on 0-row updates/deletes**
      SQL `UPDATE`/`DELETE` against a missing row returns 0 rows affected with no exception — so a silent no-op goes unnoticed today. Repo methods will check `ExecuteAsync`'s return value and throw a domain exception when it's 0; manager/controller catches normally.
      Files: all three repos' update/delete methods, new `src/PayrollCalculator.Domain/Exceptions/RecordNotFoundException.cs`.
      Notes:

## Medium-Priority

- [ ] **T4 — Plumb `CancellationToken` through async repo methods**
      Add optional `CancellationToken ct = default` on every contract method; forward via Dapper's `CommandDefinition(sql, parameters, cancellationToken: ct)`.
      Notes:

- [ ] **T5 — Replace `SELECT *` with explicit column lists**
      Fragile against schema drift, obscures intent. Affects all read queries across the three repositories.
      Notes:

- [ ] **T6 — `PayrollStatus` enum instead of `string`**
      [IPayrollRepository.cs:10](../src/PayrollCalculator.Repositories/Contracts/IPayrollRepository.cs#L10) accepts `string status` — typos compile. Define enum in `PayrollCalculator.Domain`, map to/from string at the persistence boundary.
      Notes:

- [ ] **T7 — Pagination on list queries**
      [`CompanyRepository.GetAllAsync`](../src/PayrollCalculator.Repositories/CompanyRepository.cs#L21), [`EmployeeRepository.GetByCompanyIdAsync`](../src/PayrollCalculator.Repositories/EmployeeRepository.cs#L21), [`PayrollRepository.GetByCompanyIdAsync`](../src/PayrollCalculator.Repositories/PayrollRepository.cs#L23), [`GetPayslipsByPayrollIdAsync`](../src/PayrollCalculator.Repositories/PayrollRepository.cs#L63) return unbounded sets. Add `skip`/`take` or a `Page` parameter.
      Notes:

## Low-Priority

- [ ] **T8 — Batch inserts for payslip line items**
      [PayrollManager.cs:109-127](../src/PayrollCalculator.Managers/PayrollManager.cs#L109) loops per line item — one round-trip each. Dapper's `ExecuteAsync` accepts `IEnumerable<T>` for batched parameterised inserts. Depends on T1 (needs the transaction).
      Notes:

- [ ] **T9 — `await using` for async connection disposal**
      `SqlConnection` implements `IAsyncDisposable`. Switch `using var c = ...` to `await using var c = ...` (requires exposing `DbConnection` from the factory or a cast).
      Notes:

- [ ] **T10 — Soft delete for `Company` / `Employee`**
      Hard deletes break audit trails referenced by historical payslips. Add `IsDeleted` + `DeletedAt` columns and filter in reads.
      Notes:

---

## Recommended order

T1 → T3 → T2 → T6 → T4 → T5 → T7 → T8 → T9 → T10
