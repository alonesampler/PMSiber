using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PMS.Application.Validation;

public class ValidEmailAttribute : ValidationAttribute
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var email = value.ToString();
        if (string.IsNullOrWhiteSpace(email))
            return new ValidationResult("Email обязателен для заполнения");

        if (!EmailRegex.IsMatch(email))
            return new ValidationResult("Некорректный формат email");

        return ValidationResult.Success;
    }
}
