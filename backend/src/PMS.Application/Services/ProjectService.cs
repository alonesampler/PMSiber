using FluentResults;
using PMS.Application.DTOs.Projects;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;
using PMS.Domain.Errors;

namespace PMS.Application.Services;

public class ProjectService(IUnitOfWork UnitOfWork) : IProjectService
{
    private readonly string _dataRootPath = Path.GetFullPath(Path.Combine(
        Directory.GetCurrentDirectory(),
        "..", "..", "..", "data"));

    public async Task<Result<ProjectResponseDto>> CreateAsync(ProjectUpsertDto dto)
    {
        if (dto.Params.StartDate >= dto.Params.EndDate)
            return Result.Fail(AppError.InvalidDates);

        var manager = await UnitOfWork.EmployeeRepository.GetByIdAsync(dto.ManagerId);
        if (manager == null)
            return Result.Fail(AppError.ManagerNotFound);

        if (dto.EmployeesIds.Contains(dto.ManagerId))
            return Result.Fail(AppError.ManagerInEmployees);

        var employees = new List<Employee>();
        foreach (var employeeId in dto.EmployeesIds)
        {
            var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return Result.Fail(AppError.EmployeeNotFoundById(employeeId));

            employees.Add(employee);
        }

        var project = ProjectFactory.CollectFromDto(dto);

        SetPrivateProperty(project, "Manager", manager);

        foreach (var employee in employees)
            project.AddEmployee(employee);

        await UnitOfWork.ProjectRepository.CreateAsync(project);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok(project.ToResponseDto());
    }

    public async Task<Result<ProjectResponseDto>> GetByIdAsync(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            return Result.Fail(AppError.ProjectNotFound);

        return Result.Ok(project.ToResponseDto());
    }

    public async Task<Result<List<ProjectResponseDto>>> GetAllWithFiltersAsync(
        string? name = null,
        string? customerCompanyName = null,
        string? executorCompanyName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null)
    {
        var projects = await UnitOfWork.ProjectRepository.GetAllWithFiltersAsync(
            name, customerCompanyName, executorCompanyName, startDateFrom, startDateTo);

        return Result.Ok(projects.Select(p => p.ToResponseDto()).ToList());
    }

    public async Task<Result> UpdateAsync(Guid id, ProjectUpsertDto dto)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            return Result.Fail(AppError.ProjectNotFound);

        if (dto.Params.StartDate >= dto.Params.EndDate)
            return Result.Fail(AppError.InvalidDates);

        var manager = await UnitOfWork.EmployeeRepository.GetByIdAsync(dto.ManagerId);
        if (manager == null)
            return Result.Fail(AppError.ManagerNotFound);

        if (dto.EmployeesIds.Contains(dto.ManagerId))
            return Result.Fail(AppError.ManagerInEmployees);

        var employees = new List<Employee>();
        foreach (var employeeId in dto.EmployeesIds)
        {
            var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return Result.Fail(AppError.EmployeeNotFoundById(employeeId));

            employees.Add(employee);
        }

        project.Update(
            dto.Params.Name,
            dto.Params.CustomerCompanyName,
            dto.Params.ExecutorCompanyName,
            dto.Params.StartDate,
            dto.Params.EndDate,
            dto.Params.Priority);

        project.ChangeManager(manager);

        ClearEmployees(project);
        foreach (var employee in employees)
            project.AddEmployee(employee);

        await UnitOfWork.ProjectRepository.UpdateAsync(project);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            return Result.Fail(AppError.ProjectNotFound);

        var projectFolder = Path.Combine(_dataRootPath, "uploads", "projects", id.ToString());
        if (Directory.Exists(projectFolder))
            try { Directory.Delete(projectFolder, recursive: true); }
            catch { }

        await UnitOfWork.ProjectRepository.DeleteAsync(project);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    private void SetPrivateProperty<T>(T obj, string propertyName, object value)
    {
        var prop = typeof(T).GetProperty(
            propertyName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        prop?.SetValue(obj, value);
    }

    private void ClearEmployees(Project project)
    {
        var field = typeof(Project).GetField(
            "_employees",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field?.GetValue(project) is List<Employee> list)
            list.Clear();
    }
}