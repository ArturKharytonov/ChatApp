using ChatApp.Domain.DTOs.FileDto;
using ChatApp.Domain.DTOs.Http;

namespace ChatApp.Application.Services.FileService.Interface;

public interface IFileService
{
    Task<List<FileDto>> GetFilesFromUser(string id);
    Task<bool> UploadFileAsync(UploadingRequestDto uploading);
    Task DeleteFile(string fileId);
}