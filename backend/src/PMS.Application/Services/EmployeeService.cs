using PMS.Application.DTOs.Employees;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Abstractions;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;
using PMS.Domain.ValueObjects;

namespace PMS.Application.Services;

public class EmployeeService(IUnitOfWork UnitOfWork) : IEmployeeService
{
    public async Task CreateAsync(EmployeeParamsDto dto)
    {
        // Проверяем уникальность email
        var existing = await UnitOfWork.EmployeeRepository.GetByEmailAsync(dto.Email);

        if (existing is not null)
            throw new DomainException("Сотрудник с таким email уже существует");

        // Создаем Value Objects
        var fullName = FullName.Create(dto.FirstName, dto.LastName, dto.MiddleName);
        var email = new Email(dto.Email);

        // Создаем сотрудника через конструктор
        var employee = Employee.Create(Guid.CreateVersion7(), fullName, email);

        await UnitOfWork.EmployeeRepository.CreateAsync(employee);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(Guid id)
    {
        var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(id);

        if (employee == null)
            throw new DomainException("Сотрудник не найден");

        return employee.ToResponseDto();
    }

    public async Task UpdateAsync(Guid id, EmployeeParamsDto dto)
    {
        var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(id);
        if (employee == null)
            throw new DomainException("Сотрудник не найден");

        // Проверяем уникальность email
        if (employee.Email.Value != dto.Email)
        {
            var existing = await UnitOfWork.EmployeeRepository.GetByEmailAsync(dto.Email);

            if (existing is not null)
                throw new DomainException("Сотрудник с таким email уже существует");
        }

        // Создаем новые Value Objects
        var newFullName = FullName.Create(dto.FirstName, dto.LastName, dto.MiddleName);
        var newEmail = new Email(dto.Email);

        // Используем метод Update сущности
        employee.Update(newFullName, newEmail);

        await UnitOfWork.EmployeeRepository.UpdateAsync(employee);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(id);

        if (employee is null)
            throw new DomainException("Сотрудник не найден");

        // Проверяем, не является ли сотрудник менеджером проектов
        if (employee.ManagedProjects.Any())
            throw new DomainException("Нельзя удалить сотрудника, который является менеджером проектов");

        await UnitOfWork.EmployeeRepository.DeleteAsync(employee);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await UnitOfWork.EmployeeRepository.GetAllAsync();

        return employees.ToResponseDtos();
    }

    public async Task<IEnumerable<EmployeeResponseDto>> SearchAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync();

        var employees = await UnitOfWork.EmployeeRepository.SearchAsync(query);

        return employees.ToResponseDtos();
    }
}
