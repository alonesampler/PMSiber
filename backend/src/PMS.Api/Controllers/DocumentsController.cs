using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Documents;
using PMS.Application.Interfaces;

namespace PMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IDocumentService DocumentService) : ControllerBase
{
    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var result = await DocumentService.GetByProjectAsync(projectId);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await DocumentService.GetByIdAsync(id);

        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentDto dto)
    {
        var file = Request.Form.Files.FirstOrDefault();
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var uploadDto = new UploadDocumentDto
        {
            ProjectId = dto.ProjectId,
            FileName = !string.IsNullOrWhiteSpace(dto.FileName) ? dto.FileName : file.FileName,
            ContentType = !string.IsNullOrWhiteSpace(dto.ContentType) ? dto.ContentType : file.ContentType,
            FileSize = dto.FileSize > 0 ? dto.FileSize : file.Length
        };

        await using var stream = file.OpenReadStream();
        var result = await DocumentService.UploadAsync(uploadDto, stream);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value
        );
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var downloadResult = await DocumentService.DownloadAsync(id);

        if (downloadResult.IsFailed)
            return NotFound(downloadResult.Errors);

        var documentResult = await DocumentService.GetByIdAsync(id);

        if (documentResult.IsFailed)
            return NotFound(documentResult.Errors);

        return File(
            downloadResult.Value,
            documentResult.Value.ContentType,
            documentResult.Value.FileName
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await DocumentService.DeleteAsync(id);

        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }
}