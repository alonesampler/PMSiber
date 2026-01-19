using PMS.Application.DTOs.Projects;
using PMS.Domain.Entities;

namespace PMS.Application.Factories;

public static class ProjectFactory
{
    public static ProjectResponseDto ToResponseDto(this Project project)
    {
        var startDate = project.StartDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc)
            : project.StartDate;

        var endDate = project.EndDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc)
            : project.EndDate;

        return new ProjectResponseDto(
            project.Id,
            project.Name,
            project.CustomerCompanyName,
            project.ExecutorCompanyName,
            startDate,
            endDate,
            project.Priority,
            project.Manager!.ToResponseDto(),
            project.Employees.ToResponseDtos().ToList()
        );
    }

    public static ProjectListDto ToListDto(this Project project)
    {
        var managerName = project.Manager is null
            ? string.Empty
            : $"{project.Manager.FullName.LastName} {project.Manager.FullName.FirstName}";

        var startDate = project.StartDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc)
            : project.StartDate;

        var endDate = project.EndDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc)
            : project.EndDate;

        return new ProjectListDto(
            project.Id,
            project.Name,
            project.CustomerCompanyName,
            startDate,
            endDate,
            project.Priority,
            managerName,
            project.Employees.Count
        );
    }
}