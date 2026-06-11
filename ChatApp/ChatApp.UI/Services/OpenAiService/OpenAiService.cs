using ChatApp.Infrastructure.Assistant;
using ChatApp.UI.Services.OpenAiService.Interfaces;
using HigLabo.OpenAI;
using System.Net;
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


    public async Task DeleteAssistant(string assistantId)
    {
        var vectorStoreId = await GetVectorStoreIdForAssistantAsync(assistantId);
        var response = await _httpClient.DeleteAsync($"https://api.openai.com/v1/assistants/{assistantId}");
        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
            response.EnsureSuccessStatusCode();

        if (!string.IsNullOrWhiteSpace(vectorStoreId))
        {
            var deleteVectorStoreResponse = await _httpClient.DeleteAsync($"https://api.openai.com/v1/vector_stores/{vectorStoreId}");
            if (!deleteVectorStoreResponse.IsSuccessStatusCode &&
                deleteVectorStoreResponse.StatusCode != HttpStatusCode.NotFound)
            {
                deleteVectorStoreResponse.EnsureSuccessStatusCode();
            }
        }
    }

    public async Task<string> CreateAssistantAsync(string roomName)
    {
        var assistantPayload = new
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

        var assistantResponse = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/assistants", assistantPayload);
        var assistantContent = await assistantResponse.Content.ReadAsStringAsync();
        EnsureSuccessWithDetails(assistantResponse, assistantContent, "CreateAssistantAsync.CreateAssistant");

        using var assistantJson = JsonDocument.Parse(assistantContent);
        var assistantId = assistantJson.RootElement.GetProperty("id").GetString() ?? string.Empty;

        var vectorStorePayload = new
        {
            name = $"{roomName} vector store",
            expires_after = new
            {
                anchor = "last_active_at",
                days = 14
            }
        };

        var vectorStoreResponse = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/vector_stores", vectorStorePayload);
        var vectorStoreContent = await vectorStoreResponse.Content.ReadAsStringAsync();
        EnsureSuccessWithDetails(vectorStoreResponse, vectorStoreContent, "CreateAssistantAsync.CreateVectorStore");

        using var vectorStoreJson = JsonDocument.Parse(vectorStoreContent);
        var vectorStoreId = vectorStoreJson.RootElement.GetProperty("id").GetString() ?? string.Empty;

        var updateAssistantPayload = new
        {
            tool_resources = new
            {
                file_search = new
                {
                    vector_store_ids = new[] { vectorStoreId }
                }
            }
        };
        var updateAssistantResponse = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/assistants/{assistantId}", updateAssistantPayload);
        var updateAssistantContent = await updateAssistantResponse.Content.ReadAsStringAsync();
        EnsureSuccessWithDetails(updateAssistantResponse, updateAssistantContent, "CreateAssistantAsync.AttachVectorStore");

        return assistantId;
    }

    public async Task<string> SendMessageAsync(string threadId, string message)
    {
        var payload = new
        {
            role = "user",
            content = new object[]
            {
                new
                {
                    type = "text",
                    text = message
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/threads/{threadId}/messages", payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Fallback for clients/accounts expecting plain string content.
            var fallbackPayload = new
            {
                role = "user",
                content = message
            };

            response = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/threads/{threadId}/messages", fallbackPayload);
            content = await response.Content.ReadAsStringAsync();
        }

        EnsureSuccessWithDetails(response, content, "SendMessageAsync");

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

    public async Task<string> CreateRunAsync(string assistantId, string threadId)
    {
        var payload = new
        {
            assistant_id = assistantId
        };

        var response = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/threads/{threadId}/runs", payload);
        var content = await response.Content.ReadAsStringAsync();
        EnsureSuccessWithDetails(response, content, "CreateRunAsync");

        using var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("id").GetString() ?? string.Empty;
    }

    public async Task<string> UploadFile(FileUploadParameter parameter, string assistantId)
    {
        var res = await _client.FileUploadAsync(parameter);
        var vectorStoreId = await GetVectorStoreIdForAssistantAsync(assistantId);
        if (string.IsNullOrWhiteSpace(vectorStoreId))
            throw new HttpRequestException("UploadFile failed: assistant has no vector store attached.");

        var attachPayload = new { file_id = res.Id };
        var attachResponse = await _httpClient.PostAsJsonAsync($"https://api.openai.com/v1/vector_stores/{vectorStoreId}/files", attachPayload);
        var attachResponsePayload = await attachResponse.Content.ReadAsStringAsync();
        EnsureSuccessWithDetails(attachResponse, attachResponsePayload, "UploadFile.AttachToVectorStore");

        using var attachJson = JsonDocument.Parse(attachResponsePayload);
        var vectorStoreFileId = attachJson.RootElement.GetProperty("id").GetString() ?? string.Empty;
        await WaitForVectorStoreFileReadyAsync(vectorStoreId, vectorStoreFileId);

        return res.Id;
    }

    public async Task DeleteFileFromAssistant(string assistantId, string fileId)
    {
        var vectorStoreId = await GetVectorStoreIdForAssistantAsync(assistantId);
        if (!string.IsNullOrWhiteSpace(vectorStoreId))
        {
            var vectorStoreFileResponse = await _httpClient.GetAsync($"https://api.openai.com/v1/vector_stores/{vectorStoreId}/files/{fileId}");
            if (vectorStoreFileResponse.IsSuccessStatusCode)
            {
                var vectorStoreFilePayload = await vectorStoreFileResponse.Content.ReadAsStringAsync();
                using var vectorStoreFileJson = JsonDocument.Parse(vectorStoreFilePayload);
                var vectorStoreFileId = vectorStoreFileJson.RootElement.GetProperty("id").GetString() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(vectorStoreFileId))
                {
                    var deleteVectorStoreFileResponse =
                        await _httpClient.DeleteAsync($"https://api.openai.com/v1/vector_stores/{vectorStoreId}/files/{vectorStoreFileId}");
                    if (!deleteVectorStoreFileResponse.IsSuccessStatusCode &&
                        deleteVectorStoreFileResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        deleteVectorStoreFileResponse.EnsureSuccessStatusCode();
                    }
                }
            }
        }

        var url = $"https://api.openai.com/v1/files/{fileId}";
        var fileResponse = await _httpClient.DeleteAsync(url);
        fileResponse.EnsureSuccessStatusCode();
    }

    private async Task<string?> GetVectorStoreIdForAssistantAsync(string assistantId)
    {
        var assistantResponse = await _httpClient.GetAsync($"https://api.openai.com/v1/assistants/{assistantId}");
        var assistantPayload = await assistantResponse.Content.ReadAsStringAsync();
        EnsureSuccessWithDetails(assistantResponse, assistantPayload, "GetVectorStoreIdForAssistantAsync");

        using var assistantJson = JsonDocument.Parse(assistantPayload);
        if (!assistantJson.RootElement.TryGetProperty("tool_resources", out var toolResources))
            return null;

        if (!toolResources.TryGetProperty("file_search", out var fileSearch))
            return null;

        if (!fileSearch.TryGetProperty("vector_store_ids", out var vectorStoreIds))
            return null;

        foreach (var id in vectorStoreIds.EnumerateArray())
        {
            return id.GetString();
        }

        return null;
    }

    private async Task WaitForVectorStoreFileReadyAsync(string vectorStoreId, string vectorStoreFileId)
    {
        if (string.IsNullOrWhiteSpace(vectorStoreFileId))
            return;

        const int maxAttempts = 120;
        const int delayMs = 1000;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            await Task.Delay(delayMs);
            var response = await _httpClient.GetAsync($"https://api.openai.com/v1/vector_stores/{vectorStoreId}/files/{vectorStoreFileId}");
            var payload = await response.Content.ReadAsStringAsync();
            EnsureSuccessWithDetails(response, payload, "WaitForVectorStoreFileReadyAsync");

            using var json = JsonDocument.Parse(payload);
            var status = json.RootElement.TryGetProperty("status", out var statusElement)
                ? statusElement.GetString()
                : null;

            if (status == "completed")
                return;

            if (status == "failed" || status == "cancelled")
                throw new HttpRequestException($"Vector store file processing failed: status={status}");
        }
    }

    private static void EnsureSuccessWithDetails(HttpResponseMessage response, string payload, string operationName)
    {
        if (response.IsSuccessStatusCode)
            return;

        var message = $"{operationName} failed with {(int)response.StatusCode} ({response.StatusCode}).";
        string? errorCode = null;

        try
        {
            using var json = JsonDocument.Parse(payload);
            if (json.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var errorMessage))
            {
                message += $" {errorMessage.GetString()}";
            }

            if (json.RootElement.TryGetProperty("error", out var errorObject) &&
                errorObject.TryGetProperty("code", out var codeElement))
            {
                errorCode = codeElement.GetString();
            }
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(payload))
                message += $" Payload: {payload}";
        }

        if (string.Equals(errorCode, "rate_limit_exceeded", StringComparison.OrdinalIgnoreCase))
        {
            message =
                "OpenAI quota exceeded. Please check your OpenAI plan/billing and retry later.";
        }

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}