namespace PMS.Application.DTOs.Documents;

public sealed record UploadDocumentDto(
    Guid ProjectId,
    string FileName,
    string ContentType,
    long FileSize);
