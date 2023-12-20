using HigLabo.OpenAI;

namespace OpenAIAssistent;

public class OpenAIAssistant
{

    private const string ApiKey = "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";

    public readonly OpenAIClient Client;

    public OpenAIAssistant()
    {
        Client = new OpenAIClient(ApiKey);
    }
    public async Task<string> SendMessageAsync(string threadId, string content)
    {
        var parameter = new MessageCreateParameter
        {
            Thread_Id = threadId,
            Role = "user",
            Content = content
        };

        var result = await Client.MessageCreateAsync(parameter);
        return result.Id;
    }
    public async Task<string> CreateAssistantAsync(string name, string instructions)
    {
        var parameter = new AssistantCreateParameter
        {
            Name = name,
            Instructions = instructions,
            Model = "gpt-3.5-turbo-1106",
            Tools = new List<ToolObject> { new("code_interpreter"), new("retrieval") }
        };

        var result = await Client.AssistantCreateAsync(parameter);
        return result.Id;
    }

    public async Task<string> CreateThreadAsync()
    {
        var result = await Client.ThreadCreateAsync();
        return result.Id;
    }

    public async Task<string> CreateRunAsync(string assistantId, string threadId)
    {
        var parameter = new RunCreateParameter
        {
            Assistant_Id = assistantId,
            Thread_Id = threadId
        };

        var result = await Client.RunCreateAsync(parameter);
        return result.Id;
    }
}