using PMS.Domain.Abstractions;

namespace PMS.Domain.ValueObjects;

public class FullName : ValueObject
{
    private FullName(string firstName, string lastName, string? middleName)
    {

        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName {  get; }

    public static FullName Create(string firstName, string lastName, string? middleName)
    {
        if(string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required");

        return new FullName(firstName, lastName, middleName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}
