using PMS.Application.DTOs.Projects;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Abstractions;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;

namespace PMS.Application.Services;

public class ProjectService(IUnitOfWork UnitOfWork) : IProjectService
{
    public async Task<ProjectResponseDto> CreateAsync(ProjectUpsertDto dto)
    {
        if (dto.Params.StartDate >= dto.Params.EndDate)
            throw new DomainException(
                "Дата начала должна быть раньше даты окончания",
                "PROJECT_DATE_RANGE_INVALID"
            );

        var manager = await UnitOfWork.EmployeeRepository.GetByIdAsync(dto.ManagerId);
        if (manager == null)
            throw new DomainException(
                "Менеджер не найден",
                "PROJECT_MANAGER_NOT_FOUND"
            );

        if (dto.EmployeesIds.Contains(dto.ManagerId))
            throw new DomainException(
                "Менеджер не может быть в списке исполнителей",
                "PROJECT_MANAGER_AS_EMPLOYEE"
            );

        var employees = new List<Employee>();
        foreach (var employeeId in dto.EmployeesIds)
        {
            var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new DomainException(
                    $"Сотрудник с ID {employeeId} не найден",
                    "EMPLOYEE_NOT_FOUND"
                );

            employees.Add(employee);
        }

        var project = Project.Create(
            Guid.CreateVersion7(),
            dto.Params.Name,
            dto.Params.CustomerCompanyName,
            dto.Params.ExecutorCompanyName,
            dto.Params.StartDate,
            dto.Params.EndDate,
            dto.Params.Priority,
            manager.Id);

        SetPrivateProperty(project, "Manager", manager);

        foreach (var employee in employees)
            project.AddEmployee(employee);

        await UnitOfWork.ProjectRepository.CreateAsync(project);
        await UnitOfWork.SaveChangesAsync();

        return project.ToResponseDto();
    }

    public async Task<ProjectResponseDto> GetByIdAsync(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            throw new DomainException(
                "Проект не найден",
                "PROJECT_NOT_FOUND"
            );

        return project.ToResponseDto();
    }

    public async Task<List<ProjectResponseDto>> GetAllWithFiltersAsync(
        string? name = null,
        string? customerCompanyName = null,
        string? executorCompanyName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null)
    {
        var projects = await UnitOfWork.ProjectRepository.GetAllWithFiltersAsync(
            name, customerCompanyName, executorCompanyName, startDateFrom, startDateTo);

        return projects.Select(p => p.ToResponseDto()).ToList();
    }

    public async Task UpdateAsync(Guid id, ProjectUpsertDto dto)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            throw new DomainException(
                "Проект не найден",
                "PROJECT_NOT_FOUND"
            );

        if (dto.Params.StartDate >= dto.Params.EndDate)
            throw new DomainException(
                "Дата начала должна быть раньше даты окончания",
                "PROJECT_DATE_RANGE_INVALID"
            );

        var manager = await UnitOfWork.EmployeeRepository.GetByIdAsync(dto.ManagerId);
        if (manager == null)
            throw new DomainException(
                "Менеджер не найден",
                "PROJECT_MANAGER_NOT_FOUND"
            );

        if (dto.EmployeesIds.Contains(dto.ManagerId))
            throw new DomainException(
                "Менеджер не может быть в списке исполнителей",
                "PROJECT_MANAGER_AS_EMPLOYEE"
            );

        var employees = new List<Employee>();
        foreach (var employeeId in dto.EmployeesIds)
        {
            var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new DomainException(
                    $"Сотрудник с ID {employeeId} не найден",
                    "EMPLOYEE_NOT_FOUND"
                );

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
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            throw new DomainException(
                "Проект не найден",
                "PROJECT_NOT_FOUND"
            );

        if (project.Documents.Any())
            throw new DomainException(
                "Нельзя удалить проект с документами",
                "PROJECT_HAS_DOCUMENTS"
            );

        await UnitOfWork.ProjectRepository.DeleteAsync(project);
        await UnitOfWork.SaveChangesAsync();
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
