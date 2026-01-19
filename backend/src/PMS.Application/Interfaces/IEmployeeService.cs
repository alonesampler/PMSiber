using PMS.Application.DTOs.Employees;

namespace PMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<EmployeeResponseDto>> GetAllAsync();
    Task<IEnumerable<EmployeeResponseDto>> SearchAsync(string? query);
    Task<EmployeeResponseDto> CreateAsync(EmployeeParamsDto dto);
    Task UpdateAsync(Guid id, EmployeeParamsDto dto);
    Task DeleteAsync(Guid id);
}
