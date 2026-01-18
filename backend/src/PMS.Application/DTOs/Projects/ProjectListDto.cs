using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Application.DTOs.Projects;

public class ProjectListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CustomerCompanyName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int Priority { get; init; }
    public string ManagerName { get; init; } = string.Empty;
    public int EmployeesCount { get; init; }
    public bool IsActive { get; init; }
}
