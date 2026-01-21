namespace PMS.Api.Contracts;

public class ErrorResponse(string code,string message)
{
    internal string Code { get; } = code;
    internal string Message { get; } = message;
}
