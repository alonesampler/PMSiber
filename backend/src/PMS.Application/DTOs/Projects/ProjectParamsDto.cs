using PMS.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace PMS.Application.DTOs.Projects;

public class ProjectParamsDto
{
    [Required(ErrorMessage = "Название проекта обязательно")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Название проекта должно быть от 3 до 200 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Название компании-заказчика обязательно")]
    [StringLength(100, ErrorMessage = "Название компании не должно превышать 100 символов")]
    public string CustomerCompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Название компании-исполнителя обязательно")]
    [StringLength(100, ErrorMessage = "Название компании не должно превышать 100 символов")]
    public string ExecutorCompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Дата начала обязательна")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Дата окончания обязательна")]
    [DateAfter(nameof(StartDate), ErrorMessage = "Дата окончания должна быть позже даты начала")]
    public DateTime EndDate { get; set; }

    [Range(1, 10, ErrorMessage = "Приоритет должен быть от 1 до 10")]
    public int Priority { get; set; }
}