using FluentResults;
using PMS.Application.DTOs.Projects;

namespace PMS.Application.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectResponseDto>> CreateAsync(
        ProjectUpsertDto dto);

    Task<Result<ProjectResponseDto>> GetByIdAsync(Guid id);

    Task<Result<List<ProjectResponseDto>>> GetAllWithFiltersAsync(
        string? name = null,
        string? customerCompanyName = null,
        string? executorCompanyName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null);

    Task<Result> UpdateAsync(
        Guid id,
        ProjectUpsertDto dto);

    Task<Result> DeleteAsync(Guid id);
}
