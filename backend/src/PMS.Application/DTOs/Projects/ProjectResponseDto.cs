using PMS.Application.DTOs.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Application.DTOs.Projects;

public class ProjectResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CustomerCompanyName { get; init; } = string.Empty;
    public string ExecutorCompanyName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int Priority { get; init; }
    public EmployeeResponseDto Manager { get; init; } = null!;
    public List<EmployeeResponseDto> Employees { get; init; } = new();
}

