using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Projects;
using PMS.Application.Interfaces;

namespace PMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectService ProjectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll(
        [FromQuery] string? name = null,
        [FromQuery] string? customerCompanyName = null,
        [FromQuery] string? executorCompanyName = null,
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null)
    {
        var projects = await ProjectService.GetAllWithFiltersAsync(
            name, customerCompanyName, executorCompanyName, startDateFrom, startDateTo);

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(Guid id)
    {
        var project = await ProjectService.GetByIdAsync(id);
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var project = await ProjectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
    {
        await ProjectService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await ProjectService.DeleteAsync(id);
        return NoContent();
    }
}
