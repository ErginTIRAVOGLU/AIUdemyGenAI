using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var credential = new ApiKeyCredential(configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("GitHub Models token not found in configuration."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IChatClient client = new OpenAIClient(credential, options)
    .GetChatClient("openai/gpt-5-mini").AsIChatClient();

string prompt = "What is AI ? explain max 20 words.";
Console.WriteLine($"user >>> {prompt}");

ChatResponse response = await client.GetResponseAsync(prompt);

Console.WriteLine($"assistant >>> {response}");
Console.WriteLine($"Tokens used: in={response.Usage.InputTokenCount}, out={response.Usage.OutputTokenCount}, total={response.Usage.TotalTokenCount}");