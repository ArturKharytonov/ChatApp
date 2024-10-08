﻿using ChatApp.Domain.DTOs.FileDto;
using ChatApp.Domain.DTOs.RoomDto;
using ChatApp.Infrastructure.Assistant;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using HigLabo.OpenAI;
using OpenAI.Files;
using OpenAIClient = HigLabo.OpenAI.OpenAIClient;

namespace ChatApp.UI.Services.OpenAiService;

public class OpenAiService : IOpenAiService
{
    private const string ApiKey = "YOUR-API-KEY";
    private const string _model = "gpt-3.5-turbo-1106";
    private readonly OpenAIClient _client;

    public Run Run { get; set; }

    public OpenAiService()
    {
        _client = new OpenAIClient(ApiKey);
        Run = new Run(_client);
    }

    public async Task<string> ChatCompletionAsync(string message)
    {
        message +=
            "\n\nI will provide you html , and you have to extract information about product in this html and return me IMAGE AND NAME IN JSON.\r\n\r\n!!!U can find needed info here class=\"a-section a-spacing-base\"\r\n";
        var p = new ChatCompletionsParameter
        {
            Messages = new List<ChatMessage>{new(ChatMessageRole.User, message)},
            Model = "gpt-3.5-turbo"
        };
        var res = await _client.ChatCompletionsAsync(p);

        return res.Choices[0].Message.Content;
    }

    public async Task DeleteAssistant(string assistantId)
    {
        var url = $"https://api.openai.com/v1/assistants/{assistantId}";
        _client.HttpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $"{ApiKey}");
        _client.HttpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        var re = await _client.HttpClient.DeleteAsync(url);
    }

    public async Task<string> CreateAssistantAsync(string roomName)
    {
        var parameters = new AssistantCreateParameter
        {
            Model = _model,
            Name = $"{roomName} assistant",
            Instructions = $"You are assistant {roomName} group. Please give well-structured answers. Also pay attention to files, if they exist",
            Tools = new List<ToolObject> {new("code_interpreter"), new("retrieval") }
        };

        var result = await _client.AssistantCreateAsync(parameters);
        return result.Id;
    }

    public async Task<string> SendMessageAsync(string threadId, string message)
    {
        var request = new MessageCreateParameter
        {
            Content = message,
            Role = "user",
            Thread_Id = threadId
        };

        var result = await _client.MessageCreateAsync(request);
        return result.Id;
    }

    public async Task<string> CreateThreadAsync()
    {
        var result = await _client.ThreadCreateAsync();
        return result.Id;
    }

    public async Task DeleteThreadAsync(string threadId)
    {
        await _client.ThreadDeleteAsync(threadId);
    }

    public async Task<string> CreateRunAsync(string assistantId, string threadId)
    {
        var result = await _client.RunCreateAsync(threadId, assistantId);
        return result.Id;
    }

    public async Task<AssistantRetrieveResponse> GetAssistant(string id)
    {
        return await _client.AssistantRetrieveAsync(id);
    }

    public async Task<string> UploadFile(FileUploadParameter parameter, string assistantId)
    {
        var res = await _client.FileUploadAsync(parameter);
        await _client.AssistantFileCreateAsync(assistantId, res.Id);
        return res.Id;
    }

    public async Task DeleteFileFromAssistant(string assistantId, string fileId)
    {
        var urlForAssistant = $"https://api.openai.com/v1/assistants/{assistantId}/files/{fileId}";
        var url = $"https://api.openai.com/v1/files/{fileId}";

        _client.HttpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $"{ApiKey}");
        _client.HttpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");

        await _client.HttpClient.DeleteAsync(urlForAssistant);
        await _client.HttpClient.DeleteAsync(url);
    }
}