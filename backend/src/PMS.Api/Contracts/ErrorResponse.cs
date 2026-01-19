namespace PMS.Api.Contracts;

public sealed record ErrorResponse(
    string Code,
    string Message
);
