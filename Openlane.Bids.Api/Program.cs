using Serilog.Events;
using Serilog.Formatting.Compact;
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

// Serilog configuration for structured logging
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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

//Map Routes
app.MapRoutes();

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json";
    await next();
});

app.Run();


[JsonSerializable(typeof(Result<string>))]
[JsonSerializable(typeof(CreateBid))]
[JsonSerializable(typeof(CreatedBid))]
[JsonSerializable(typeof(IEnumerable<CreatedBid>))]
[JsonSerializable(typeof(ProblemDetails))]
internal partial class ResultJsonContext:JsonSerializerContext
{
}