using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.Application.DTOs.Projects;

public class ProjectParamsDto
{
    [Required(ErrorMessage = "Название проекта обязательно")]
    [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Компания-заказчик обязательна")]
    [StringLength(200, ErrorMessage = "Название компании не должно превышать 200 символов")]
    public string CustomerCompanyName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Компания-исполнитель обязательна")]
    [StringLength(200, ErrorMessage = "Название компании не должно превышать 200 символов")]
    public string ExecutorCompanyName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Дата начала обязательна")]
    public DateTime StartDate { get; init; }

    [Required(ErrorMessage = "Дата окончания обязательна")]
    public DateTime EndDate { get; init; }

    [Range(1, 10, ErrorMessage = "Приоритет должен быть от 1 до 10")]
    public int Priority { get; init; } = 5;
}