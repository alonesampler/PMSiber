using PMS.Application.DTOs.Documents;
using PMS.Domain.Entities;

namespace PMS.Application.Factories;

public static class DocumentFactory
{
    public static DocumentResponseDto ToResponseDto(this Document document, string baseUrl = "")
    {
        if (document == null) return null!;

        return new DocumentResponseDto
        {
            Id = document.Id,
            FileName = document.FileName,
            DownloadUrl = !string.IsNullOrEmpty(baseUrl)
                ? $"{baseUrl}/api/documents/{document.Id}/download"
                : string.Empty,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            UploadDate = document.UploadDate
        };
    }

    public static IEnumerable<DocumentResponseDto> ToResponseDtos(
        this IEnumerable<Document> documents,
        string baseUrl = "")
    {
        return documents.Select(d => d.ToResponseDto(baseUrl));
    }
}
