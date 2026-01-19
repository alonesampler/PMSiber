using Microsoft.EntityFrameworkCore;
using PMS.Api.Extensions;
using PMS.Application;
using PMS.Infrastructure;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// configure loggingr
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// configure controllers and JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
    });

// configure layers
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// configure custom services and configurations
builder.Services
    .AddHttpContextAccessor()
    .AddCorsConfiguration(builder.Configuration)
    .AddSwaggerConfiguration();

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// conver Middleware
app.UseApiMiddlewares();

// development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PMS API v1");
        options.RoutePrefix = "api-docs";
        options.DocumentTitle = "PMS API Documentation";
    });

    // other dev-specific middleware
    app.UseDeveloperExceptionPage();
}

// production
if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// main middleware
app.UseCors("Frontend");
app.UseAuthentication(); // if add authentication
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();