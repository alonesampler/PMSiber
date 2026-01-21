namespace PMS.Api.Contracts;

public class ValidationErrorResponse : ErrorResponse
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationErrorResponse(
        string code,
        string message,
        Dictionary<string, string[]> errors)
        : base(code, message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
