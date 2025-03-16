using Serilog;
using Openlane.Bids.Shared.Extensions;
using Openlane.Bids.Api;
using Openlane.Bids;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Openlane.Bids.Api.Validators;
using Openlane.Bids.Api.Dtos;
using Openlane.Bids.Api.Endpoints;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using Serilog.Events;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, BidJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(1, ResultJsonContext.Default);
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var logLevel = builder.Configuration.GetValue<string>("Serilog:MinimumLevel:Default") ?? "Information";
if (!Enum.TryParse(logLevel, true, out LogEventLevel level))
{
    level = LogEventLevel.Information;
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(level) // Apply log level from config
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Override for Microsoft logs
    .MinimumLevel.Override("System", LogEventLevel.Warning)    // Override for System logs
    .Enrich.FromLogContext()
    .WriteTo.Console() // Console sink
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // File sink
    .CreateLogger();

builder.Logging.ClearProviders(); // Clear default logging
builder.Host.UseSerilog(); // Use Serilog

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minimal API with OpenAPI",
        Version = "v1",
        Description = "Bids Api",
        Contact = new OpenApiContact
        {
            Name = "sachin Chauhan",
            Email = "sachinchauhan.cs1988@gmail.com"
        }
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddScoped<IValidator<CreateBid>, CreateBidValidator>();

builder.Services.AddScoped<BidController>();

//Sql Server
builder.Services.AddRepository();
// RabbitMQ
await builder.Services.AddQueueService();
//Redis Cache
builder.Services.AddCacheService();

var app = builder.Build();

// ✅ Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" || context.Request.Path == "/index.html")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

//Map Routes
app.MapRoutes();

app.UseSerilogRequestLogging();
app.Run();


[JsonSerializable(typeof(Result<string>))]
[JsonSerializable(typeof(CreateBid))]
[JsonSerializable(typeof(CreatedBid))]
[JsonSerializable(typeof(IEnumerable<CreatedBid>))]
[JsonSerializable(typeof(ProblemDetails))]
internal partial class ResultJsonContext:JsonSerializerContext
{
}