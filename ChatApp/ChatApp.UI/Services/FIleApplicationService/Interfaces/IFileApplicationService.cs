using ChatApp.Domain.DTOs.Http.Responses;
using ChatApp.Domain.DTOs.Http;

namespace ChatApp.UI.Services.FIleApplicationService.Interfaces;

public interface IFileApplicationService
{
    Task<UploadingResponseDto> AddFileToRoomAsync(UploadingRequestDto uploading);
    Task<FilesFromUserResponseDto> GetAllFilesFromSpecificUser(string id);
    Task DeleteFile(string fileId);
}