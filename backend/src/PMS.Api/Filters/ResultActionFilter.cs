using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.Api.Contracts; // Убедитесь, что есть этот using
using PMS.Domain.Errors;
using System.Text.Json;

namespace PMS.Api.Filters;

public class ResultActionFilter : IAsyncActionFilter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // ModelState валидация работает автоматически с атрибутами
        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelState(context);
            return;
        }

        var executedContext = await next();

        // Обработка Result из сервисов
        if (executedContext.Result is ObjectResult objectResult &&
            objectResult.Value != null)
        {
            var resultType = objectResult.Value.GetType();

            if (typeof(IResultBase).IsAssignableFrom(resultType))
            {
                HandleResult(executedContext, resultType, (IResultBase)objectResult.Value);
            }
        }
    }

    private static void HandleInvalidModelState(ActionExecutingContext context)
    {
        var errors = context.ModelState
            .Where(ms => ms.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => ToCamelCase(kvp.Key),
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).Distinct().ToArray()
            );

        var errorResponse = new ValidationErrorResponse(
            "VALIDATION_ERROR",
            "Ошибка валидации",
            errors
        );

        context.Result = new BadRequestObjectResult(errorResponse);
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    private static void HandleResult(
        ActionExecutedContext context,
        Type resultType,
        IResultBase result)
    {
        if (result.IsSuccess)
        {
            HandleSuccessResult(context, resultType, result);
        }
        else
        {
            HandleFailedResult(context, result);
        }
    }

    private static void HandleSuccessResult(
        ActionExecutedContext context,
        Type resultType,
        IResultBase result)
    {
        if (resultType.IsGenericType &&
            resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueProperty = resultType.GetProperty("Value");
            if (valueProperty != null)
            {
                var value = valueProperty.GetValue(result);
                context.Result = new OkObjectResult(value);
            }
            else
            {
                context.Result = new OkResult();
            }
        }
        else
        {
            context.Result = new OkObjectResult(result);
        }
    }

    private static void HandleFailedResult(
        ActionExecutedContext context,
        IResultBase result)
    {
        var firstError = result.Errors.FirstOrDefault();
        var (code, message) = GetErrorInfo(firstError);
        var statusCode = MapErrorToStatusCode(code);

        var errorResponse = new ErrorResponse(code, message);
        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = statusCode
        };
    }

    private static (string code, string message) GetErrorInfo(IError error)
    {
        if (error is AppError appError)
        {
            return (appError.Code, appError.Message);
        }

        if (error is Error fluentError && fluentError.Metadata.TryGetValue("Code", out var codeObj))
        {
            var code = codeObj as string ?? "UNKNOWN_ERROR";
            return (code, error.Message);
        }

        return ("UNKNOWN_ERROR", error?.Message ?? "Произошла ошибка");
    }

    private static int MapErrorToStatusCode(string errorCode)
    {
        return errorCode switch
        {
            // Employee
            "EMPLOYEE_NOT_FOUND" => StatusCodes.Status404NotFound,
            "EMPLOYEE_EMAIL_ALREADY_EXISTS" => StatusCodes.Status409Conflict,
            "EMPLOYEE_IS_MANAGER" => StatusCodes.Status409Conflict,
            "MANAGER_NOT_FOUND" => StatusCodes.Status404NotFound,

            // Project
            "PROJECT_NOT_FOUND" => StatusCodes.Status404NotFound,
            "INVALID_DATES" => StatusCodes.Status400BadRequest,
            "PROJECT_HAS_DOCUMENTS" => StatusCodes.Status409Conflict,
            "MANAGER_IN_EMPLOYEES" => StatusCodes.Status400BadRequest,

            // Document
            "DOCUMENT_NOT_FOUND" => StatusCodes.Status404NotFound,
            "FILE_TOO_LARGE" => StatusCodes.Status400BadRequest,
            "INVALID_EXTENSION" => StatusCodes.Status400BadRequest,
            "FILE_MISSING" => StatusCodes.Status404NotFound,

            // DEFAULT
            _ => StatusCodes.Status400BadRequest
        };
    }
}