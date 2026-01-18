using PMS.Application.DTOs.Employees;
using PMS.Domain.Entities;
using PMS.Domain.ValueObjects;

namespace PMS.Application.Factories;

public static class EmployeeFactory
{
    public static EmployeeResponseDto ToResponseDto(this Employee employee)
    {
        if (employee == null) return null!;

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FullName.FirstName,
            LastName = employee.FullName.LastName,
            MiddleName = employee.FullName.MiddleName,
            Email = employee.Email.Value
        };
    }

    public static IEnumerable<EmployeeResponseDto> ToResponseDtos(this IEnumerable<Employee> employees)
    {
        return employees.Select(ToResponseDto);
    }
}
