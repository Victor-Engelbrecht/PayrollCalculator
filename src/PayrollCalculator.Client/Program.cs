using PayrollCalculator.Adapters;
using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Engines;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories;
using PayrollCalculator.Repositories.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();

builder.Services.AddScoped<ICompanyManager, CompanyManager>();
builder.Services.AddScoped<IEmployeeManager, EmployeeManager>();
builder.Services.AddScoped<IPayrollManager, PayrollManager>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<IPaymentAdapter, PaymentProviderAdapter>();
builder.Services.AddScoped<IEmailAdapter, EmailAdapter>();
builder.Services.AddScoped<IPayCalculationEngine, PayCalculationEngine>();
builder.Services.AddSingleton<PayrollRuleFactory>();

var app = builder.Build();

var dbFactory = app.Services.GetRequiredService<IDbConnectionFactory>();
await new DatabaseInitializer(dbFactory).InitializeAsync();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
