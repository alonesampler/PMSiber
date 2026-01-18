using Microsoft.EntityFrameworkCore.Storage;
using PMS.Domain.Interfaces;

namespace PMS.Infrastructure.EfCore.Repositories;

public class UnitOfWork(
    AppDbContext DbContext,
    IProjectRepository projectRepository,
    IEmployeeRepository employeeRepository,
    IDocumentRepository documentRepository
    ) : IUnitOfWork
{
    private IDbContextTransaction _currentTransaction;

    public IProjectRepository ProjectRepository { get; } = projectRepository;
    public IEmployeeRepository EmployeeRepository { get; } = employeeRepository;
    public IDocumentRepository DocumentRepository { get; } = documentRepository;

    public void Dispose()
        => DbContext.Dispose();

    public Task SaveChangesAsync(bool applySoftDeleted = true)
        => DbContext.SaveChangesAsync(applySoftDeleted);

    public bool HasActiveTransaction
        => _currentTransaction is not null;


    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTransaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await DbContext.SaveChangesAsync();

            await _currentTransaction?.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException("A transaction must be in progress to execute rollback.");

        try
        {
            await _currentTransaction.RollbackAsync();
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

}
