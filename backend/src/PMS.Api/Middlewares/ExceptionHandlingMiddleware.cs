using Microsoft.AspNetCore.Http;
using PMS.Api.Contracts;
using PMS.Domain.Abstractions;
using System.Text.Json;

namespace PMS.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = MapStatusCode(ex.ErrorCode);
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse(ex.ErrorCode, ex.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse(
                "INTERNAL_ERROR",
                "Внутренняя ошибка сервера"
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
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
