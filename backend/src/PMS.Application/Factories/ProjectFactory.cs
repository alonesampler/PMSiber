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

    public static Project CollectFromDto(ProjectUpsertDto dto)
    {
            var project = Project.Create(
            Guid.CreateVersion7(),
            dto.Params.Name,
            dto.Params.CustomerCompanyName,
            dto.Params.ExecutorCompanyName,
            dto.Params.StartDate,
            dto.Params.EndDate,
            dto.Params.Priority,
            dto.ManagerId);

        return project;
    }
}