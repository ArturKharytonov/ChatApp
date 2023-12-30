using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Infrastructure.Assistant;
using HigLabo.OpenAI;

namespace ChatApp.UI.Services.OpenAiService.Interfaces;

public interface IOpenAiService
{
    Run Run { get; set; }
    Task DeleteAssistant(string assistantId);
    Task DeleteFileFromAssistant(string assistantId, string fileId);
    Task UploadFileToAssistant(RoomDto room);
    Task<string> CreateAssistantAsync(string roomName);
    Task<string> SendMessageAsync(string threadId, string message);
    Task<string> CreateThreadAsync();
    Task DeleteThreadAsync(string threadId);
    Task<string> CreateRunAsync(string assistantId, string threadId);
    Task<string> UploadFile(FileUploadParameter parameter);
    Task<string> ChatCompletionAsync(string message);
}