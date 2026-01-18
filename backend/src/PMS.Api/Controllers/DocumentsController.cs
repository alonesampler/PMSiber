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

        var uploadDto = new UploadDocumentDto
        {
            ProjectId = dto.ProjectId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length
        };

        await using var stream = file.OpenReadStream();
        var document = await DocumentService.UploadAsync(uploadDto, stream);

        return CreatedAtAction(nameof(GetById), new { id = document.Id }, document);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<ActionResult> Download(Guid id)
    {
        var fileStream = await DocumentService.DownloadAsync(id);

        // Получаем документ для имени файла
        var document = await DocumentService.GetByIdAsync(id);

        return File(fileStream, "application/octet-stream", document?.FileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await DocumentService.DeleteAsync(id);
        return NoContent();
    }
}