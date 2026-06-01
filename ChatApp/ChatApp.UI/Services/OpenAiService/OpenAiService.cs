using ChatApp.Infrastructure.Assistant;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using HigLabo.OpenAI;
using OpenAI.Files;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using OpenAIClient = HigLabo.OpenAI.OpenAIClient;

namespace ChatApp.UI.Services.OpenAiService;

public class OpenAiService : IOpenAiService
{
    private const string ApiKeyFallback = "";
    private const string _model = "gpt-4o-mini";
    private readonly OpenAIClient _client;
    private readonly HttpClient _httpClient;

    public Run Run { get; set; }

    public OpenAiService()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = ApiKeyFallback;

        _client = new OpenAIClient(apiKey);
        _httpClient = _client.HttpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Remove("OpenAI-Beta");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("OpenAI-Beta", "assistants=v2");
        Run = new Run(_httpClient);
    }

    public async Task<string> ChatCompletionAsync(string message)
    {
        message +=
            "\n\nI will provide you html , and you have to extract information about product in this html and return me IMAGE AND NAME IN JSON.\r\n\r\n!!!U can find needed info here class=\"a-section a-spacing-base\"\r\n";
        var p = new ChatCompletionsParameter
        {
            Messages = new List<ChatMessage>{new(ChatMessageRole.User, message)},
            Model = _model
        };
        var res = await _client.ChatCompletionsAsync(p);

        return res.Choices[0].Message.Content;
    }

    public async Task DeleteAssistant(string assistantId)
    {
        var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/assistants/{assistantId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> CreateAssistantAsync(string roomName)
    {
        var payload = new
        {
            model = _model,
            name = $"{roomName} assistant",
            instructions = $"You are assistant {roomName} group. Please give well-structured answers. Also pay attention to files, if they exist",
            tools = new object[]
            {
                new { type = "code_interpreter" },
                new { type = "file_search" }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/assistants", payload);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("id").GetString() ?? string.Empty;
    }

    public async Task<string> SendMessageAsync(string threadId, string message)
    {
        var payload = new
        {
            role = "user",
            content = message
        };

        var response = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/threads/{threadId}/messages", payload);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("id").GetString() ?? string.Empty;
    }

    public async Task<string> CreateThreadAsync()
    {
        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/threads", new { });
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("id").GetString() ?? string.Empty;
    }

    public async Task DeleteThreadAsync(string threadId)
    {
        var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/threads/{threadId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> CreateRunAsync(string assistantId, string threadId)
    {
        var payload = new
        {
            assistant_id = assistantId
        };

        var response = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/threads/{threadId}/runs", payload);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("id").GetString() ?? string.Empty;
    }

    public async Task<string> UploadFile(FileUploadParameter parameter, string assistantId)
    {
        var res = await _client.FileUploadAsync(parameter);
        var attachPayload = new { file_id = res.Id };
        var attachResponse =
            await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/assistants/{assistantId}/files", attachPayload);

        // Some v2 accounts may not expose assistant-file attachment endpoint.
        // We keep file upload usable even if attachment isn't supported.
        if (!attachResponse.IsSuccessStatusCode && attachResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            attachResponse.EnsureSuccessStatusCode();
        }

        return res.Id;
    }

    public async Task DeleteFileFromAssistant(string assistantId, string fileId)
    {
        var assistantFileResponse = await _httpClient.DeleteAsync($"https://api.openai.com/v1/assistants/{assistantId}/files/{fileId}");
        if (!assistantFileResponse.IsSuccessStatusCode &&
            assistantFileResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            assistantFileResponse.EnsureSuccessStatusCode();
        }

        var url = $"https://api.openai.com/v1/files/{fileId}";
        var fileResponse = await _httpClient.DeleteAsync(url);
        fileResponse.EnsureSuccessStatusCode();
    }
}