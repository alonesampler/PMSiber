using PMS.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace PMS.Application.DTOs.Employees;

public class EmployeeParamsDto
{
    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 50 символов")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Фамилия обязательна")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна быть от 2 до 50 символов")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [ValidEmail(ErrorMessage = "Некорректный формат email")]
    [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
    public string Email { get; set; } = string.Empty;
}
