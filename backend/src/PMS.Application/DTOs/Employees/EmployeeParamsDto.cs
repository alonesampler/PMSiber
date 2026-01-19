namespace PMS.Application.DTOs.Employees;

public sealed record EmployeeParamsDto(
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email);
