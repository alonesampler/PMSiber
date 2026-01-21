using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PMS.Api.Contracts;
using PMS.Domain.Abstractions;
using System.Text.Json;

namespace PMS.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment environment)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleSystemExceptionAsync(context, ex, environment);
        }
    }

    private static async Task HandleDomainExceptionAsync(HttpContext context, DomainException ex)
    {
        context.Response.StatusCode = MapStatusCode(ex.ErrorCode);
        context.Response.ContentType = "application/json";

        var payload = new ErrorResponse(ex.ErrorCode, ex.Message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static async Task HandleSystemExceptionAsync(
        HttpContext context,
        Exception ex,
        IWebHostEnvironment environment)
    {
        // Логирование
        var logger = context.RequestServices.GetService<ILogger<ExceptionHandlingMiddleware>>();
        logger?.LogError(ex, "Unhandled exception");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var message = environment.IsDevelopment()
            ? ex.Message
            : "Внутренняя ошибка сервера";

        var payload = new ErrorResponse("INTERNAL_ERROR", message);

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static int MapStatusCode(string code)
    {
        return code switch
        {
            // EMPLOYEE
            "EMPLOYEE_NOT_FOUND" => StatusCodes.Status404NotFound,
            "EMPLOYEE_EMAIL_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
            "EMPLOYEE_IS_MANAGER" => StatusCodes.Status409Conflict,

            // EMAIL / VALIDATION
            "EMAIL_EMPTY" => StatusCodes.Status400BadRequest,
            "INVALID_EMAIL_FORMAT" => StatusCodes.Status400BadRequest,
            "FIRST_NAME_REQUIRED" => StatusCodes.Status400BadRequest,
            "LAST_NAME_REQUIRED" => StatusCodes.Status400BadRequest,

            // DEFAULT
            _ => StatusCodes.Status400BadRequest
        };
    }
}