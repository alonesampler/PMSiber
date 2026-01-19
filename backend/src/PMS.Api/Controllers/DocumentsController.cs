using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Documents;
using PMS.Application.Interfaces;

namespace PMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IDocumentService DocumentService) : ControllerBase
{
    [HttpGet("project/{projectId:guid}")]
    public async Task<ActionResult<IEnumerable<DocumentResponseDto>>> GetByProject(Guid projectId)
    {
        var documents = await DocumentService.GetByProjectAsync(projectId);
        return Ok(documents);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DocumentResponseDto>> GetById(Guid id)
    {
        var document = await DocumentService.GetByIdAsync(id);
        return Ok(document);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<DocumentResponseDto>> Upload([FromForm] UploadDocumentDto dto)
    {
        var file = Request.Form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var uploadDto = new UploadDocumentDto(
            dto.ProjectId,
            dto.FileName,
            dto.ContentType,
            dto.FileSize
        );

        await using var stream = file.OpenReadStream();
        var document = await DocumentService.UploadAsync(uploadDto, stream);

        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var fileStream = await DocumentService.DownloadAsync(id);
        var document = await DocumentService.GetByIdAsync(id);

        return File(
            fileStream,
            document.ContentType,
            document.FileName
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await DocumentService.DeleteAsync(id);
        return NoContent();
    }
}
