using System.ComponentModel.DataAnnotations;

namespace PMS.Application.Validation;

public class DateAfterAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateAfterAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = (DateTime?)value;
        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
            return new ValidationResult($"Unknown property: {_comparisonProperty}");

        var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

        if (currentValue.HasValue && comparisonValue.HasValue && currentValue <= comparisonValue)
        {
            return new ValidationResult($"{validationContext.DisplayName} should be later {_comparisonProperty}");
        }

        return ValidationResult.Success;
    }
}
