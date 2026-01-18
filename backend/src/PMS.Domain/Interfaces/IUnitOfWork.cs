namespace PMS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDocumentRepository DocumentRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IEmployeeRepository EmployeeRepository { get; }

    Task SaveChangesAsync(bool applySoftDeleted = true);

    bool HasActiveTransaction { get; }

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}