namespace PMS.Application.DTOs.Documents;

public class DocumentResponseDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string DownloadUrl { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public DateTime UploadDate { get; init; }
}
