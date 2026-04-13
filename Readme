# PayrollCalculator

A simple payroll management application — built for fun and as a playground for exploring AI-assisted development.

## Overview

PayrollCalculator is an ASP.NET Core Web API that handles the core concerns of running payroll for one or more companies:
managing employees, applying custom additions and deductions, disbursing payments through a payment provider,
delivering payslips by email, and maintaining a full audit trail of every payroll run.

## Tech Stack

- **Language / Runtime**: C# / .NET 8
- **API Framework**: ASP.NET Core Web API + Swagger (Swashbuckle)
- **Data Access**: Dapper + Microsoft SQL Server
- **Testing**: NUnit

## Architecture

The solution follows a layered architecture to keep concerns cleanly separated:

```
PayrollCalculator.Client        — ASP.NET Core Web API (entry point, controllers)
    └── PayrollCalculator.Managers   — Orchestration; coordinates engines and data access
            ├── PayrollCalculator.Engines    — Business / calculation logic
            ├── PayrollCalculator.Repositories  — Data access (SQL via Dapper)
            └── PayrollCalculator.Utilities  — Shared utilities (no external dependencies)
```

## Features

- **Company management** — Support multiple companies within a single deployment
- **Employee management** — Manage employees per company
- **Custom additions & deductions** — Apply flat-amount adjustments per employee per pay run
- **Payment provider integration** — Disburse payroll through a single payment provider
- **Payslip delivery** — Send payslips to employees via email
- **Audit trail** — Full record of every payroll run: who was paid, how much, and when

## Out of Scope (for now)

- Multi-currency / multi-jurisdiction tax calculations
- Payroll approval workflows
- Multiple payment provider integrations
- Additional communication channels (SMS, employee portal, etc.)

## Getting Started

```bash
# Clone the repo, then build the solution
dotnet build PayrollCalculator.sln

# Run the API
dotnet run --project src/PayrollCalculator.Client

# Run the tests
dotnet test PayrollCalculator.sln
```

> The project is in early development. A database schema and environment configuration will be added as the project matures.
