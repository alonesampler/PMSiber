using PMS.Application.DTOs.Documents;

namespace PMS.Application.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> UploadAsync(
        UploadDocumentDto request,
        Stream fileStream);

    Task DeleteAsync(Guid id);

    Task<DocumentResponseDto> GetByIdAsync(Guid id);

    Task<IEnumerable<DocumentResponseDto>> GetByProjectAsync(Guid projectId);

    Task<FileStream> DownloadAsync(Guid id);
}
