using ChatApp.Application.Services.FileService.Interface;
using ChatApp.Domain.DTOs.FileDto;
using ChatApp.Domain.DTOs.Http.Requests.Files;
using ChatApp.Domain.Mappers.Files;
using ChatApp.Persistence.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services.FileService;

public class FileService : IFileService
{
    private readonly IUnitOfWork _unitOfWork;

    public FileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> UploadFileAsync(UploadingRequestDto uploading)
    {
        if (!int.TryParse(uploading.SenderId, out var senderId) ||
            !int.TryParse(uploading.RoomId, out var roomId))
            return false;

        var file = new Domain.Files.File
        {
            Id = uploading.FileId,
            Name = uploading.FileName,
            UserId = senderId,
            GroupId = roomId
        };

        await _unitOfWork.GetRepository<Domain.Files.File, string>()!.CreateAsync(file);
        await _unitOfWork.SaveAsync();
        return true;
    }
    public async Task<List<FileDto>> GetFilesFromUser(string id)
    {
        var files = await _unitOfWork
                                        .GetRepository<Domain.Files.File, string>()!
                                        .GetAllAsQueryableAsync();
        files = files
            .Include(f => f.Group)
            .Include(f => f.User);
        if (!files.Any()) 
            return new List<FileDto>();

        var list = files
            .Where(f => f.UserId.ToString() == id)
            .Select(f => f.ToFileDto())
            .ToList();

        return list;
    }
    public async Task DeleteFile(string fileId)
    {
        await _unitOfWork.GetRepository<Domain.Files.File, string>()!.DeleteAsync(fileId);
        await _unitOfWork.SaveAsync();
    }
}