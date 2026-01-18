using PMS.Application.DTOs.Projects;
using PMS.Domain.Entities;

namespace PMS.Application.Factories;

public static class ProjectFactory
{
    public static ProjectResponseDto ToResponseDto(this Project project)
    {
        if (project == null) return null!;

        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            CustomerCompanyName = project.CustomerCompanyName,
            ExecutorCompanyName = project.ExecutorCompanyName,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Priority = project.Priority,
            Manager = project.Manager?.ToResponseDto(),
            Employees = project.Employees.ToResponseDtos().ToList()
        };
    }

    public static ProjectListDto ToListDto(this Project project)
    {
        return new ProjectListDto
        {
            Id = project.Id,
            Name = project.Name,
            CustomerCompanyName = project.CustomerCompanyName,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Priority = project.Priority,
            ManagerName = project.Manager != null
                ? $"{project.Manager.FullName.LastName} {project.Manager.FullName.FirstName}"
                : string.Empty,
            EmployeesCount = project.Employees.Count,
            IsActive = project.StartDate <= DateTime.UtcNow && project.EndDate >= DateTime.UtcNow
        };
    }
}
