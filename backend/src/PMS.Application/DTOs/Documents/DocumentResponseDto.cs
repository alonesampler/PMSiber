namespace PMS.Application.DTOs.Documents;

public sealed record DocumentResponseDto(
    Guid Id,
    string FileName,
    string DownloadUrl,
    string ContentType,
    long FileSize,
    DateTime UploadDate);
