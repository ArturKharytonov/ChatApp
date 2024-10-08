using ChatApp.Domain.DTOs.Http.Requests.Files;
using ChatApp.Domain.DTOs.Http.Responses.Files;

namespace ChatApp.UI.Services.FIleApplicationService.Interfaces;

public interface IFileApplicationService
{
    Task<UploadingResponseDto> AddFileToRoomAsync(UploadingRequestDto uploading);
    Task<FilesFromUserResponseDto> GetAllFilesFromSpecificUser(string id);
    Task DeleteFile(string fileId);
}