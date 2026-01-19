using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PMS.Application.DTOs.Documents;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Abstractions;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;

namespace PMS.Application.Services;

public class DocumentService(
    IUnitOfWork UnitOfWork,
    IHttpContextAccessor HttpContextAccessor,
    IConfiguration Configuration) : IDocumentService
{
    private readonly string _dataRootPath = Path.GetFullPath(Path.Combine(
        Directory.GetCurrentDirectory(),
        "..", "..", "..", "data"));

    public async Task<DocumentResponseDto> UploadAsync(
        UploadDocumentDto request,
        Stream fileStream)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
            throw new DomainException(
                "Проект не найден",
                "PROJECT_NOT_FOUND"
            );

        ValidateFile(request);

        var projectFolder = Path.Combine(
            _dataRootPath,
            "uploads",
            "projects",
            request.ProjectId.ToString());

        Directory.CreateDirectory(projectFolder);

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(projectFolder, uniqueFileName);

        await using (var fs = new FileStream(filePath, FileMode.Create))
            await fileStream.CopyToAsync(fs);

        var relativePath = Path.Combine(
            "uploads",
            "projects",
            request.ProjectId.ToString(),
            uniqueFileName);

        var document = Document.Create(
            Guid.CreateVersion7(),
            request.FileName,
            relativePath,
            request.ContentType,
            request.FileSize,
            request.ProjectId);

        await UnitOfWork.DocumentRepository.CreateAsync(document);
        await UnitOfWork.SaveChangesAsync();

        return document.ToResponseDto(GetBaseUrl());
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new DomainException(
                "Документ не найден",
                "DOCUMENT_NOT_FOUND"
            );

        var fullPath = Path.Combine(_dataRootPath, document.FilePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        await UnitOfWork.DocumentRepository.DeleteAsync(document);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetByProjectAsync(Guid projectId)
    {
        var documents = await UnitOfWork.DocumentRepository.GetByProjectIdAsync(projectId);
        return documents.ToResponseDtos(GetBaseUrl());
    }

    public async Task<DocumentResponseDto> GetByIdAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new DomainException(
                "Документ не найден",
                "DOCUMENT_NOT_FOUND"
            );

        return document.ToResponseDto(GetBaseUrl());
    }

    public async Task<FileStream> DownloadAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new DomainException(
                "Документ не найден",
                "DOCUMENT_NOT_FOUND"
            );

        var fullPath = Path.Combine(_dataRootPath, document.FilePath);
        if (!File.Exists(fullPath))
            throw new DomainException(
                "Физический файл не найден",
                "DOCUMENT_FILE_MISSING"
            );

        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private void ValidateFile(UploadDocumentDto request)
    {
        const long maxFileSize = 50 * 1024 * 1024;
        if (request.FileSize > maxFileSize)
            throw new DomainException(
                "Файл слишком большой",
                "DOCUMENT_FILE_TOO_LARGE"
            );

        var allowedExtensions = new[]
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"
        };

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new DomainException(
                "Недопустимое расширение файла",
                "DOCUMENT_EXTENSION_NOT_ALLOWED"
            );
    }

    private string GetBaseUrl()
    {
        var request = HttpContextAccessor.HttpContext?.Request;
        return request == null
            ? string.Empty
            : $"{request.Scheme}://{request.Host}";
    }
}
