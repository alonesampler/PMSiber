using PMS.Domain.Entities;

namespace PMS.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task CreateAsync(Employee entity);
    Task UpdateAsync(Employee entity);
    Task DeleteAsync(Employee entity);
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee[]> SearchAsync(string? q = null);
    Task<Employee[]> GetAllAsync();
}
