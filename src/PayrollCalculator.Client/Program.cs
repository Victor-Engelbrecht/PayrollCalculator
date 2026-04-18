using PayrollCalculator.Adapters;
using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Client.Logging;
using PayrollCalculator.Client.Middleware;
using PayrollCalculator.Engines;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.RuleProviders;
using PayrollCalculator.Managers;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Utilities.Contracts;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();

builder.Services.AddScoped<IWideEventContext, SerilogWideEventContext>();

builder.Services.AddScoped<ICompanyManager, CompanyManager>();
builder.Services.AddScoped<IEmployeeManager, EmployeeManager>();
builder.Services.AddScoped<IPayrollManager, PayrollManager>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<IPaymentAdapter, PaymentProviderAdapter>();
builder.Services.AddScoped<IEmailAdapter, EmailAdapter>();
builder.Services.AddScoped<IPayCalculationEngine, PayCalculationEngine>();
builder.Services.AddScoped<IPayrollNotificationEngine, PayrollNotificationEngine>();
builder.Services.AddScoped<IPayrollRuleProvider, CoreRulesProvider>();
builder.Services.AddScoped<IPayrollRuleProvider, CompanyRulesProvider>();
builder.Services.AddScoped<IPayrollRuleProvider, EmployeeRulesProvider>();
builder.Services.AddScoped<IPayrollRuleProvider, CountryRulesProvider>();
builder.Services.AddScoped<IPayrollRuleFactory, PayrollRuleFactory>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("request_id", httpContext.TraceIdentifier);

        if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.ItemKey, out var correlationId)
            && correlationId is string cid)
        {
            diagnosticContext.Set("correlation_id", cid);
        }

        diagnosticContext.Set("user_agent", httpContext.Request.Headers.UserAgent.ToString());
        diagnosticContext.Set("client_ip", httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
        diagnosticContext.Set("query_string", httpContext.Request.QueryString.Value ?? string.Empty);
        diagnosticContext.Set("content_length", httpContext.Request.ContentLength ?? 0);
        diagnosticContext.Set("response_content_type", httpContext.Response.ContentType ?? string.Empty);
    };
});

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
