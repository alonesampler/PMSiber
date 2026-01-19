using System.ComponentModel.DataAnnotations;

namespace PMS.Application.DTOs.Projects;

public sealed record ProjectParamsDto(
    string Name,
    string CustomerCompanyName,
    string ExecutorCompanyName,
    DateTime StartDate,
    DateTime EndDate,
    int Priority = 5
);