namespace PMS.Application.DTOs.Documents;

public class UploadDocumentDto
{
    public Guid ProjectId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
}
