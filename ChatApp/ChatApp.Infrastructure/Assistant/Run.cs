﻿using HigLabo.OpenAI;

namespace ChatApp.Infrastructure.Assistant
{
    public class Run
    {
        private readonly OpenAIClient _client;

        public Run(OpenAIClient client)
        {
            _client = client;
        }

        public async Task<string> RetrieveRunResultAsync(string threadId, string runId)
        {
            var loopCount = 0;
            var interval = 1000;

            while (true)
            {
                Thread.Sleep(interval);
                var parameter = new RunRetrieveParameter
                {
                    Thread_Id = threadId,
                    Run_Id = runId
                };

                var result = await _client.RunRetrieveAsync(parameter);

                if (result.Status != "queued" && result.Status != "in_progress" && result.Status != "cancelling")
                {
                    var messagesParameter = new MessagesParameter
                    {
                        Thread_Id = threadId,
                        QueryParameter = { Order = "desc" }
                    };

                    var messagesResult = await _client.MessagesAsync(messagesParameter);
                    return messagesResult.Data[0].Content[0].Text.Value;
                }

                loopCount++;
                if (loopCount > 120) { break; }
            }

            return string.Empty;
        }
    }
}
