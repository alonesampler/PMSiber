using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Application.DTOs.Employees;

public class EmployeeParamsDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string Email { get; init; } = string.Empty;
}
