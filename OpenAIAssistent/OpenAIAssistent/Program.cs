using OpenAIAssistent;

var assistant = new OpenAIAssistant();
var run = new OpenAIRun(assistant.Client);

var assistantId = await assistant.CreateAssistantAsync("ChatApp", "You are a personal assistant for my ChatApp. Please give well-structured answers");
var threadId = await assistant.CreateThreadAsync();

while (true)
{
    Console.Write("Enter message: ");
    var message = Console.ReadLine();
    if (message!.Equals("EXIT", StringComparison.CurrentCultureIgnoreCase))
        break;

    await assistant.SendMessageAsync(threadId, message);
    var runId = await assistant.CreateRunAsync(assistantId, threadId);
    var result = await run.RetrieveRunResultAsync(threadId, runId);

    Console.WriteLine("response: " + result);
}
