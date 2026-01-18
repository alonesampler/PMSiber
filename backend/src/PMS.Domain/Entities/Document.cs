using PMS.Domain.Abstractions;

namespace PMS.Domain.Entities;

public class Document : Entity<Guid>
{
    private Document() : base() { }

    private Document(
        Guid id,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        Guid projectId) : base(id)
    {
        Id = id;
        FileName = fileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;
        UploadDate = DateTime.UtcNow;
        ProjectId = projectId;
    }

    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string ContentType { get; private set; }
    public long FileSize { get; private set; }
    public DateTime UploadDate { get; private set; }

    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public static Document Create(
        Guid id,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        Guid projectId)
    {
        Validate(fileName, contentType, fileSize);

        return new Document(
            id,
            fileName,
            filePath,
            contentType,
            fileSize,
            projectId);
    }

    private static void Validate(string fileName, string contentType, long fileSize)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name is required");

        if (fileName.Length > 500)
            throw new DomainException("File name is too long");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException("Content type is required");

        if (fileSize <= 0)
            throw new DomainException("File size must be positive");

        const long MAX_SIZE = 100 * 1024 * 1024;
        if (fileSize > MAX_SIZE)
            throw new DomainException($"File size exceeds maximum of {MAX_SIZE / 1024 / 1024} MB");
    }

    public void UpdateFileName(string newFileName)
    {
        if (string.IsNullOrWhiteSpace(newFileName))
            throw new DomainException("File name is required");

        if (newFileName.Length > 500)
            throw new DomainException("File name is too long");

        FileName = newFileName;
    }

    public string GetFileExtension()
    {
        return Path.GetExtension(FileName).ToLowerInvariant();
    }

    public bool IsImage()
    {
        var imageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        return imageTypes.Contains(ContentType);
    }
}