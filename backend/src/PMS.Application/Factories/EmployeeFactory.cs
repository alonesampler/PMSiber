using PMS.Application.DTOs.Employees;
using PMS.Domain.Entities;
using PMS.Domain.ValueObjects;

namespace PMS.Application.Factories;

public static class EmployeeFactory
{
    public static EmployeeResponseDto ToResponseDto(this Employee employee)
        => new(
            employee.Id,
            employee.FullName.FirstName,
            employee.FullName.LastName,
            employee.FullName.MiddleName,
            employee.Email.Value
        );

    public static IEnumerable<EmployeeResponseDto> ToResponseDtos(this IEnumerable<Employee> employees)
        => employees.Select(ToResponseDto);
}
