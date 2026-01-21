using FluentResults;
using PMS.Application.DTOs.Documents;

namespace PMS.Application.Interfaces;

public interface IDocumentService
{
    Task<Result<DocumentResponseDto>> UploadAsync(
        UploadDocumentDto request,
        Stream fileStream);

    Task<Result> DeleteAsync(Guid id);

    Task<Result<DocumentResponseDto>> GetByIdAsync(Guid id);

    Task<Result<IEnumerable<DocumentResponseDto>>> GetByProjectAsync(Guid projectId);

    Task<Result<FileStream>> DownloadAsync(Guid id);
}
