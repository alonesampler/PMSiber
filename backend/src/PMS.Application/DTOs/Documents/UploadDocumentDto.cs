using System.ComponentModel.DataAnnotations;

namespace PMS.Application.DTOs.Documents;

public class UploadDocumentDto
{
    [Required(ErrorMessage = "ID проекта обязателен")]
    public Guid ProjectId { get; set; }

    [Required(ErrorMessage = "Имя файла обязательно")]
    [StringLength(255, ErrorMessage = "Имя файла не должно превышать 255 символов")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Тип файла обязателен")]
    public string ContentType { get; set; } = string.Empty;

    [Range(1, 50 * 1024 * 1024, ErrorMessage = "Размер файла должен быть от 1 байта до 50 МБ")]
    public long FileSize { get; set; }
}
