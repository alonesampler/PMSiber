namespace PMS.Application.DTOs.Projects;

public sealed record ProjectListDto(
    Guid Id,
    string Name,
    string CustomerCompanyName,
    DateTime StartDate,
    DateTime EndDate,
    int Priority,
    string ManagerName,
    int EmployeesCount);
