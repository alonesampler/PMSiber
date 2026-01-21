using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Employees;
using PMS.Application.Interfaces;

namespace PMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService EmployeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await EmployeeService.GetAllAsync();

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query)
    {
        var result = await EmployeeService.SearchAsync(query);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await EmployeeService.GetByIdAsync(id);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeParamsDto dto)
    {
        var result = await EmployeeService.CreateAsync(dto);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeParamsDto dto)
    {
        var result = await EmployeeService.UpdateAsync(id, dto);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await EmployeeService.DeleteAsync(id);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }
}
