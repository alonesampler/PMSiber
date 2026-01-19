using PMS.Application.DTOs.Documents;
using PMS.Domain.Entities;

namespace PMS.Application.Factories;

public static class DocumentFactory
{
    public static DocumentResponseDto ToResponseDto(this Document document, string baseUrl = "")
    {
        var url = !string.IsNullOrWhiteSpace(baseUrl)
            ? $"{baseUrl}/api/documents/{document.Id}/download"
            : string.Empty;

        return new DocumentResponseDto(
            document.Id,
            document.FileName,
            url,
            document.ContentType,
            document.FileSize,
            document.UploadDate
        );
    }

    public static IEnumerable<DocumentResponseDto> ToResponseDtos(this IEnumerable<Document> documents, string baseUrl = "")
        => documents.Select(d => d.ToResponseDto(baseUrl));
}
