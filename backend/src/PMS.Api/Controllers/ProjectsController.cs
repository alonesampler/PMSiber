using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Projects;
using PMS.Application.Interfaces;

namespace PMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectService ProjectService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name = null,
        [FromQuery] string? customerCompanyName = null,
        [FromQuery] string? executorCompanyName = null,
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null)
    {
        var result = await ProjectService.GetAllWithFiltersAsync(
            name, customerCompanyName, executorCompanyName, startDateFrom, startDateTo);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await ProjectService.GetByIdAsync(id);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectUpsertDto dto)
    {
        var result = await ProjectService.CreateAsync(dto);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectUpsertDto dto)
    {
        var result = await ProjectService.UpdateAsync(id, dto);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await ProjectService.DeleteAsync(id);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }
}