using FluentResults;
using Microsoft.AspNetCore.Http;
using PMS.Application.DTOs.Documents;
using PMS.Application.Factories;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;
using PMS.Domain.Errors;

namespace PMS.Application.Services;

public class DocumentService(
    IUnitOfWork UnitOfWork,
    IHttpContextAccessor HttpContextAccessor
) : IDocumentService
{
    private readonly string _dataRootPath = Path.GetFullPath(Path.Combine(
        Directory.GetCurrentDirectory(),
        "..", "..", "..", "data"));

    public async Task<Result<DocumentResponseDto>> UploadAsync(
        UploadDocumentDto request,
        Stream fileStream)
    {
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
            return Result.Fail(AppError.ProjectNotFoundForDocument);

        var validation = ValidateFile(request);
        if (validation.IsFailed)
            return validation.ToResult<DocumentResponseDto>();

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

        return Result.Ok(document.ToResponseDto(GetBaseUrl()));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            return Result.Fail(AppError.DocumentNotFound);

        var fullPath = Path.Combine(_dataRootPath, document.FilePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        await UnitOfWork.DocumentRepository.DeleteAsync(document);
        await UnitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<IEnumerable<DocumentResponseDto>>> GetByProjectAsync(Guid projectId)
    {
        var documents = await UnitOfWork.DocumentRepository.GetByProjectIdAsync(projectId);
        return Result.Ok(documents.ToResponseDtos(GetBaseUrl()));
    }

    public async Task<Result<DocumentResponseDto>> GetByIdAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            return Result.Fail(AppError.DocumentNotFound);

        return Result.Ok(document.ToResponseDto(GetBaseUrl()));
    }

    public async Task<Result<FileStream>> DownloadAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            return Result.Fail(AppError.DocumentNotFound);

        var fullPath = Path.Combine(_dataRootPath, document.FilePath);
        if (!File.Exists(fullPath))
            return Result.Fail(AppError.FileMissing);

        var stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return Result.Ok(stream);
    }

    private Result ValidateFile(UploadDocumentDto request)
    {
        const long maxFileSize = 50 * 1024 * 1024;
        if (request.FileSize > maxFileSize)
            return Result.Fail(AppError.FileTooLarge);

        var allowedExtensions = new[]
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"
        };

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return Result.Fail(AppError.InvalidExtension);

        return Result.Ok();
    }

    private string GetBaseUrl()
    {
        var request = HttpContextAccessor.HttpContext?.Request;
        return request == null
            ? string.Empty
            : $"{request.Scheme}://{request.Host}";
    }
}