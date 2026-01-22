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
            return new ValidationResult("Email required field");

        if (!EmailRegex.IsMatch(email))
            return new ValidationResult("Invalid format email");
            
        return ValidationResult.Success;
    }
}
