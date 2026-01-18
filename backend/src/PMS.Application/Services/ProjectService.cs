using PMS.Application.DTOs.Projects;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Abstractions;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;

namespace PMS.Application.Services;

public class ProjectService(IUnitOfWork UnitOfWork) : IProjectService
{
    public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto)
    {
        if (dto.Params.StartDate >= dto.Params.EndDate)
            throw new DomainException("Дата начала должна быть раньше даты окончания");

        var manager = await UnitOfWork.EmployeeRepository.GetByIdAsync(dto.ManagerId);
        if (manager == null)
            throw new DomainException("Менеджер не найден");

        // Получаем сотрудников
        var employees = new List<Employee>();
        foreach (var employeeId in dto.EmployeesIds)
        {
            var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new DomainException($"Сотрудник с ID {employeeId} не найден");

            employees.Add(employee);
        }

        if (dto.EmployeesIds.Contains(dto.ManagerId))
            throw new DomainException("Менеджер не может быть в списке исполнителей");

        // Создаем проект через доменый метод
        var project = Project.Create(
            Guid.CreateVersion7(),
            dto.Params.Name,
            dto.Params.CustomerCompanyName,
            dto.Params.ExecutorCompanyName,
            dto.Params.StartDate,
            dto.Params.EndDate,
            dto.Params.Priority,
            manager.Id);

        // Устанавливаем менеджера через приватное свойство
        SetPrivateProperty(project, "Manager", manager);

        // Добавляем сотрудников
        foreach (var employee in employees)
        {
            project.AddEmployee(employee);
        }

        await UnitOfWork.ProjectRepository.CreateAsync(project);
        await UnitOfWork.SaveChangesAsync();

        return project.ToResponseDto();
    }

    public async Task<ProjectResponseDto> GetByIdAsync(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            throw new DomainException("Проект не найден");

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

    public async Task UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            throw new DomainException("Проект не найден");

        if (dto.Params.StartDate >= dto.Params.EndDate)
            throw new DomainException("Дата начала должна быть раньше даты окончания");

        var manager = await UnitOfWork.EmployeeRepository.GetByIdAsync(dto.ManagerId);
        if (manager == null)
            throw new DomainException("Менеджер не найден");

        // Получаем сотрудников
        var employees = new List<Employee>();
        foreach (var employeeId in dto.EmployeesIds)
        {
            var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new DomainException($"Сотрудник с ID {employeeId} не найден");

            employees.Add(employee);
        }

        if (dto.EmployeesIds.Contains(dto.ManagerId))
            throw new DomainException("Менеджер не может быть в списке исполнителей");

        // Обновляем базовую информацию
        project.Update(
            dto.Params.Name,
            dto.Params.CustomerCompanyName,
            dto.Params.ExecutorCompanyName,
            dto.Params.StartDate,
            dto.Params.EndDate,
            dto.Params.Priority);

        // Обновляем менеджера
        project.ChangeManager(manager);

        // Очищаем и добавляем сотрудников
        ClearEmployees(project);
        foreach (var employee in employees)
        {
            project.AddEmployee(employee);
        }

        await UnitOfWork.ProjectRepository.UpdateAsync(project);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(id);
        if (project == null)
            throw new DomainException("Проект не найден");

        if (project.Documents.Any())
            throw new DomainException("Нельзя удалить проект, к которому прикреплены документы");

        await UnitOfWork.ProjectRepository.DeleteAsync(project);
        await UnitOfWork.SaveChangesAsync();
    }

    // Вспомогательные методы для работы с приватными полями
    private void SetPrivateProperty<T>(T obj, string propertyName, object value)
    {
        var property = typeof(T).GetProperty(propertyName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
        property?.SetValue(obj, value);
    }

    private void ClearEmployees(Project project)
    {
        var employeesField = typeof(Project)
            .GetField("_employees",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        if (employeesField != null)
        {
            var employeeList = employeesField.GetValue(project) as List<Employee>;
            employeeList?.Clear();
        }
    }
}