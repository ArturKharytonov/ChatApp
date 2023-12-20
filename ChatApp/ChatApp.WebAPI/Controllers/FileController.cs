using ChatApp.Application.Services.FileService.Interface;
using ChatApp.Domain.DTOs.Http.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers;

[Route("api/files")]
[ApiController]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilesFromUser([FromQuery] string id)
    {
        var files = await _fileService.GetFilesFromUser(id);

        return Ok(new FilesFromUserResponseDto { Files = files });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFile([FromQuery] string fileId)
    {
        await _fileService.DeleteFile(fileId);
        return Ok("was deleted");
    }
}