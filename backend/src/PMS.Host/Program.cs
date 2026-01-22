using Microsoft.EntityFrameworkCore;
using PMS.Api.Extensions;
using PMS.Api.Filters;
using PMS.Application;
using PMS.Host.Extensions;
using PMS.Infrastructure;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResultActionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
});

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddHttpContextAccessor()
    .AddCorsConfiguration(builder.Configuration)
    .AddSwaggerConfiguration();

builder.Services.AddHealthChecks();

var app = builder.Build();

AutoMigrator.ApplyMigrations(app);

app.UseApiMiddlewares();

app.UseSwagger();
app.UseSwaggerUI();

app.UseDeveloperExceptionPage();

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();