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
        "..", "..", "data"));

    public async Task<DocumentResponseDto> UploadAsync(
        UploadDocumentDto request,
        Stream fileStream)
    {
        // Проверяем существование проекта
        var project = await UnitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
            throw new DomainException($"Проект с ID {request.ProjectId} не найден");

        // Валидация файла
        ValidateFile(request);

        // Создаем папку проекта
        var projectFolder = Path.Combine(_dataRootPath, "uploads", "projects", request.ProjectId.ToString());
        Directory.CreateDirectory(projectFolder);

        // Генерируем уникальное имя файла
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(projectFolder, uniqueFileName);

        // Сохраняем файл
        await using (var fs = new FileStream(filePath, FileMode.Create))
            await fileStream.CopyToAsync(fs);

        // Относительный путь (от data)
        var relativePath = Path.Combine("uploads", "projects", request.ProjectId.ToString(), uniqueFileName);

        // Создаем документ через конструктор
        var document = Document.Create(
            Guid.CreateVersion7(),
            request.FileName,
            relativePath,
            request.ContentType,
            request.FileSize,
            request.ProjectId);

        // Сохраняем
        await UnitOfWork.DocumentRepository.CreateAsync(document);
        await UnitOfWork.SaveChangesAsync();

        // Генерируем URL для скачивания
        var baseUrl = GetBaseUrl();
        return document.ToResponseDto(baseUrl);
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new DomainException("Документ не найден");

        // Удаляем физический файл
        var fullPath = Path.Combine(_dataRootPath, document.FilePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        // Удаляем из БД
        await UnitOfWork.DocumentRepository.DeleteAsync(document);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<DocumentResponseDto>> GetByProjectAsync(Guid projectId)
    {
        var documents = await UnitOfWork.DocumentRepository.GetByProjectIdAsync(projectId);
        var baseUrl = GetBaseUrl();
        return documents.ToResponseDtos(baseUrl);
    }

    public async Task<DocumentResponseDto> GetByIdAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new DomainException("Документ не найден");

        var baseUrl = GetBaseUrl();
        return document.ToResponseDto(baseUrl);
    }

    public async Task<FileStream> DownloadAsync(Guid id)
    {
        var document = await UnitOfWork.DocumentRepository.GetByIdAsync(id);
        if (document == null)
            throw new DomainException("Документ не найден");

        var fullPath = Path.Combine(_dataRootPath, document.FilePath);
        if (!File.Exists(fullPath))
            throw new DomainException("Физический файл не найден");

        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    private void ValidateFile(UploadDocumentDto request)
    {
        const long maxFileSize = 50 * 1024 * 1024; // 50 MB
        if (request.FileSize > maxFileSize)
            throw new DomainException($"Файл слишком большой. Максимум: {maxFileSize / 1024 / 1024} MB");

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" };
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new DomainException($"Недопустимое расширение. Разрешены: {string.Join(", ", allowedExtensions)}");
    }

    private string GetBaseUrl()
    {
        var request = HttpContextAccessor.HttpContext?.Request;
        if (request == null) return string.Empty;

        return $"{request.Scheme}://{request.Host}";
    }
}