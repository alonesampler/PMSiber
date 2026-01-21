using FluentResults;
using PMS.Application.DTOs.Employees;

namespace PMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<Result<EmployeeResponseDto?>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<EmployeeResponseDto>>> GetAllAsync();
    Task<Result<IEnumerable<EmployeeResponseDto>>> SearchAsync(string? query);
    Task<Result<EmployeeResponseDto>> CreateAsync(EmployeeParamsDto dto);
    Task<Result> UpdateAsync(Guid id, EmployeeParamsDto dto);
    Task<Result> DeleteAsync(Guid id);
}
