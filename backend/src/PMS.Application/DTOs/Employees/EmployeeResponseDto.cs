namespace PMS.Application.DTOs.Employees;

public sealed record EmployeeResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddleName,
    string Email);
