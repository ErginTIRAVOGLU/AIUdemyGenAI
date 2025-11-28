using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// create a chat client
IChatClient client =
    new OpenAIClient(credential, options).GetChatClient("openai/gpt-5-mini").AsIChatClient();

foreach (var imagePath  in Directory.GetFiles("images", "*.jpg"))
{
    var name = Path.GetFileNameWithoutExtension(imagePath);

    var message = new ChatMessage(ChatRole.User, $$"""
    Extract information from this image from camera "{{name}}".
        Respond with JSON object in this form: {
            "Status": string // One of these values: "Clear", "Flowing", "Congested", "Blocked",
            "NumCars": number,
            "NumTrucks": number
        }
    """);

    message.Contents.Add(new DataContent(File.ReadAllBytes(imagePath), "image/jpeg"));

    var response = await client.GetResponseAsync<TrafficCamResult>([message]);

    if(response.TryGetResult(out var result))
    {
        Console.WriteLine($"{name}: Status={result.Status}, Cars={result.NumCars}, Trucks={result.NumTrucks}");
    }
}