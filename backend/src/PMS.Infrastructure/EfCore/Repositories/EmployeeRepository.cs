using Microsoft.EntityFrameworkCore;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;

namespace PMS.Infrastructure.EfCore.Repositories;

public class EmployeeRepository(AppDbContext DbContext) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(Guid id)
        => DbContext.Employees
            .Include(e => e.ManagedProjects)
            .Include(e => e.AssignedProjects)
            .FirstOrDefaultAsync(e => e.Id == id);

    public Task<Employee?> GetByEmailAsync(string email)
        => DbContext.Employees.FirstOrDefaultAsync(e => e.Email.Value == email);

    public  Task<Employee[]> GetAllAsync()
        =>  DbContext.Employees
            .Include(e => e.ManagedProjects)
            .Include(e => e.AssignedProjects)
            .ToArrayAsync();

    public Task<Employee[]> SearchAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAllAsync();

        return DbContext.Employees
            .Where(e => e.FullName.FirstName.Contains(query) ||
                       e.FullName.LastName.Contains(query) ||
                       (e.FullName.MiddleName != null && e.FullName.MiddleName.Contains(query)) ||
                       e.Email.Value.Contains(query))
            .Include(e => e.ManagedProjects)
            .Include(e => e.AssignedProjects)
            .ToArrayAsync();
    }

    public Task CreateAsync(Employee employee)
         => DbContext.Employees.AddAsync(employee).AsTask();

    public Task UpdateAsync(Employee employee)
    {
        DbContext.Employees.Update(employee);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Employee employee)
    {
        DbContext.Employees.Remove(employee);
        return Task.CompletedTask;
    }
}
