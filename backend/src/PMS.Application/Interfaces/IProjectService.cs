using PMS.Application.DTOs.Projects;

namespace PMS.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectResponseDto> CreateAsync(
        CreateProjectDto dto);

    Task<ProjectResponseDto> GetByIdAsync(Guid id);

    Task<List<ProjectResponseDto>> GetAllWithFiltersAsync(
        string? name = null,
        string? customerCompanyName = null,
        string? executorCompanyName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null);

    Task UpdateAsync(
        Guid id,
        UpdateProjectDto dto);

    Task DeleteAsync(Guid id);
}
