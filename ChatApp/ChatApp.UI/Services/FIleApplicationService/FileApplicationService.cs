using ChatApp.UI.Services.FIleApplicationService.Interfaces;
using ChatApp.UI.Services.HttpClientPWAService;
using ChatApp.UI.Services.HttpClientPWAService.Interfaces;
using ChatApp.Domain.DTOs.Http.Responses.Files;
using ChatApp.Domain.DTOs.Http.Requests.Files;

namespace ChatApp.UI.Services.FIleApplicationService;

public class FileApplicationService : IFileApplicationService
{
    private readonly IHttpClientPwa _clientPwa;

    public FileApplicationService(IHttpClientPwa clientPwa)
    {
        _clientPwa = clientPwa;
    }
    public async Task<UploadingResponseDto> AddFileToRoomAsync(UploadingRequestDto uploading)
    {
        var result = await _clientPwa.PostAsync<UploadingRequestDto, UploadingResponseDto>(HttpClientPwa.UploadFileToRoom, uploading);
        return result.Result;
    }

    public async Task<FilesFromUserResponseDto> GetAllFilesFromSpecificUser(string id)
    {
        var result = await _clientPwa.GetAsync<FilesFromUserResponseDto>(HttpClientPwa.GetFilesFromUser + "id=" + id);
        return result.Result;
    }

    public async Task DeleteFile(string fileId)
    {
        await _clientPwa.DeleteAsync(HttpClientPwa.DeleteFile + "fileId=" + fileId);
    }
}