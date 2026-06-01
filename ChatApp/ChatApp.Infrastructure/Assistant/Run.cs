using System.Text.Json;

namespace ChatApp.Infrastructure.Assistant
{
    public class Run
    {
        private readonly HttpClient _httpClient;

        public Run(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RetrieveRunResultAsync(string threadId, string runId)
        {
            const int maxAttempts = 120;
            const int delayMs = 1000;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                await Task.Delay(delayMs);

                var runResponse = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
                var runPayload = await runResponse.Content.ReadAsStringAsync();
                runResponse.EnsureSuccessStatusCode();

                using var runJson = JsonDocument.Parse(runPayload);
                var status = runJson.RootElement.GetProperty("status").GetString();

                if (status is "failed" or "cancelled" or "expired")
                    return string.Empty;

                if (status is "completed")
                {
                    var messageResponse =
                        await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages?order=desc&limit=20");
                    var messagePayload = await messageResponse.Content.ReadAsStringAsync();
                    messageResponse.EnsureSuccessStatusCode();

                    using var messagesJson = JsonDocument.Parse(messagePayload);
                    var data = messagesJson.RootElement.GetProperty("data");

                    foreach (var message in data.EnumerateArray())
                    {
                        if (!message.TryGetProperty("role", out var roleElement) ||
                            roleElement.GetString() != "assistant")
                        {
                            continue;
                        }

                        if (!message.TryGetProperty("content", out var contentElement))
                            continue;

                        foreach (var contentItem in contentElement.EnumerateArray())
                        {
                            if (!contentItem.TryGetProperty("type", out var typeElement) ||
                                typeElement.GetString() != "text")
                            {
                                continue;
                            }

                            if (!contentItem.TryGetProperty("text", out var textElement))
                                continue;

                            if (!textElement.TryGetProperty("value", out var valueElement))
                                continue;

                            return valueElement.GetString() ?? string.Empty;
                        }
                    }

                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
