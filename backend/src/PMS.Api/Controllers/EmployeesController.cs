using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Employees;
using PMS.Application.Interfaces;

namespace PMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService EmployeeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetAll()
    {
        var employees = await EmployeeService.GetAllAsync();
        return Ok(employees);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> Search([FromQuery] string? query)
    {
        var employees = await EmployeeService.SearchAsync(query);
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(Guid id)
    {
        var employee = await EmployeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] EmployeeParamsDto dto)
    {
        await EmployeeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] EmployeeParamsDto dto)
    {
        await EmployeeService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await EmployeeService.DeleteAsync(id);
        return NoContent();
    }
}
