namespace PMS.Domain.Abstractions;

public class DomainException : Exception
{
    /// <summary>
    /// Unic code error
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Other data error
    /// </summary>
    public object? Details { get; }

    public DomainException(string message, string errorCode, object? details = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Details = details;
    }

    public DomainException(string message)
        : base(message)
    {
        ErrorCode = "ERROR";
    }
}
