using PMS.Application.DTOs.Employees;

namespace PMS.Application.DTOs.Projects;

public sealed record ProjectResponseDto(
    Guid Id,
    string Name,
    string CustomerCompanyName,
    string ExecutorCompanyName,
    DateTime StartDate,
    DateTime EndDate,
    int Priority,
    EmployeeResponseDto Manager,
    List<EmployeeResponseDto> Employees);

