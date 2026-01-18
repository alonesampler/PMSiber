using Microsoft.EntityFrameworkCore;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;

namespace PMS.Infrastructure.EfCore.Repositories;

public class DocumentRepository(AppDbContext DbContext) : IDocumentRepository
{
    public Task<Document?> GetByIdAsync(Guid id)
        => DbContext.Documents
            .Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.Id == id);

    public Task<Document[]> GetByProjectIdAsync(Guid projectId)
        => DbContext.Documents
            .Where(d => d.ProjectId == projectId)
            .ToArrayAsync();

    public Task CreateAsync(Document document)
        => DbContext.Documents.AddAsync(document).AsTask();

    public Task UpdateAsync(Document document)
    {
        DbContext.Documents.Update(document);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Document document)
    {
        DbContext.Documents.Remove(document);
        return Task.CompletedTask;
    }
}
