using FluentResults;
using PMS.Application.DTOs.Employees;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;
using PMS.Domain.ValueObjects;
using PMS.Domain.Errors;

namespace PMS.Application.Services;

public class EmployeeService(IUnitOfWork UnitOfWork) : IEmployeeService
{
    public async Task<Result<EmployeeResponseDto>> CreateAsync(EmployeeParamsDto dto)
    {
        var existing = await UnitOfWork.EmployeeRepository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return Result.Fail(AppError.EmployeeEmailExists);

        var fullName = FullName.Create(dto.FirstName, dto.LastName, dto.MiddleName);
        var email = new Email(dto.Email);

        var employee = Employee.Create(Guid.CreateVersion7(), fullName, email);

        await UnitOfWork.EmployeeRepository.CreateAsync(employee);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok(employee.ToResponseDto());
    }

    public async Task<Result<EmployeeResponseDto>> GetByIdAsync(Guid id)
    {
        var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(id);
        if (employee is null)
            return Result.Fail(AppError.EmployeeNotFound);

        return Result.Ok(employee.ToResponseDto());
    }

    public async Task<Result> UpdateAsync(Guid id, EmployeeParamsDto dto)
    {
        var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(id);
        if (employee is null)
            return Result.Fail(AppError.EmployeeNotFound);

        if (employee.Email.Value != dto.Email)
        {
            var existing = await UnitOfWork.EmployeeRepository.GetByEmailAsync(dto.Email);
            if (existing is not null)
                return Result.Fail(AppError.EmployeeEmailExists);
        }

        var fullName = FullName.Create(dto.FirstName, dto.LastName, dto.MiddleName);
        var email = new Email(dto.Email);

        employee.Update(fullName, email);

        await UnitOfWork.EmployeeRepository.UpdateAsync(employee);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var employee = await UnitOfWork.EmployeeRepository.GetByIdAsync(id);
        if (employee is null)
            return Result.Fail(AppError.EmployeeNotFound);

        if (employee.ManagedProjects.Any())
            return Result.Fail(AppError.EmployeeIsManager);

        await UnitOfWork.EmployeeRepository.DeleteAsync(employee);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<IEnumerable<EmployeeResponseDto>>> GetAllAsync()
    {
        var employees = await UnitOfWork.EmployeeRepository.GetAllAsync();
        return Result.Ok(employees.ToResponseDtos());
    }

    public async Task<Result<IEnumerable<EmployeeResponseDto>>> SearchAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync();

        var employees = await UnitOfWork.EmployeeRepository.SearchAsync(query);
        return Result.Ok(employees.ToResponseDtos());
    }
}