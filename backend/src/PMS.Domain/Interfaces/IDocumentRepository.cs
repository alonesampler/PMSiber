using PMS.Domain.Entities;

namespace PMS.Domain.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id);
    Task<Document[]> GetByProjectIdAsync(Guid projectId);
    Task CreateAsync(Document document);
    Task DeleteAsync(Document document);
}
