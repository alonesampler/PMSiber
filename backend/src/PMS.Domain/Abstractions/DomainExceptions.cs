namespace PMS.Domain.Abstractions;

public class DomainException : Exception
{
    public string ErrorCode { get; }

    public DomainException(string message)
        : base(message)
    {
        ErrorCode = "DOMAIN_ERROR";
    }

    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
