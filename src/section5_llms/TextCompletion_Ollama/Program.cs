

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");


#region Basic Text Completion
/*
string prompt = "What is AI ? explain max 20 words.";
Console.WriteLine($"user >>> {prompt}");

ChatResponse response = await client.GetResponseAsync(prompt);

Console.WriteLine($"assistant >>> {response}");
Console.WriteLine($"Tokens used: in={response.Usage.InputTokenCount}, out={response.Usage.OutputTokenCount}, total={response.Usage.TotalTokenCount}");
*/
#endregion


#region Sentiment Analysis

/*
var analysisPrompt = """
You will analyze the sentiment of the following product reviews.
Each line is its own review. Output the sentiment of each review in a bulleted list as 'positive', 'negative', or 'neutral' then provide a generate sentiment of all reviews.

I bought this product and it's amazing. I love it!
This product is terrible. I hate it.
I'm not sure about this product. It's okay.
I found this product based on the other reviews. It worked for a bit, and the
""";
Console.WriteLine($"user >>> {analysisPrompt}");
ChatResponse analysisResponse = await client.GetResponseAsync(analysisPrompt);
Console.WriteLine($"assistant >>> \n {analysisResponse}");
*/
#endregion


#region Classification
/*
var classificationPrompt = """
Classify the following sentences into categories:
- 'complaint'
- 'suggestion'
- 'praise'
- 'other'.

1) "I love the new layout!"
2) "You should add a night mode."
3) "When I try to log in, it keeps failing."
4) "This app is decent."

""";
Console.WriteLine($"user >>> {classificationPrompt}");
ChatResponse classificationResponse = await client.GetResponseAsync(classificationPrompt);
Console.WriteLine($"assistant >>> \n {classificationResponse}");
*/
#endregion

#region  Structured Output
/*
var carListings = new []
{
    "Check out this stylish 2019 Toyota Camry. It has a clean title, only 40,000 miles on the odometer.",
    "Lease this sporty 2021 Honda Civic! With only 10,000 miles, it includes a sunroof, premium sound system, and more.",
    "A classic 1968 Ford Mustang, perfect for enthusiasts. The vehicle needs some interior restoration.",
    "Brand new 2023 Tesla Model 3 for lease. Zero miles, fully electric, autopilot capabilities, and long-range battery.",
    "Selling a 2015 Subaru Outback in good condition. 60,000 miles on it, includes all-wheel drive."
};

foreach(var listingText in carListings)
{
    var response = await client.GetResponseAsync<CarDetails>(
        $"""
        Convert the following car listing into a JSON object matching this C# schema:
        Condition: "New" or "Used"
        Make: (car manufacturer)
        Model: (car model)
        Year: (four-digit year)
        ListingType: "Sale" or "Lease"
        Price: integer only
        Features: array of short strings
        TenWordSummary: exactly ten words to summarize this listing

        Here is the listing:
        {listingText}
        """);
    
    if(response.TryGetResult(out var info))
    {
        Console.WriteLine(JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
    }
    else
    {
        Console.WriteLine("Repsonse was not in the expected format.");
    }
}

class CarDetails
{
    public required string Condition { get; set; } //e.g. new, used
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public CarListingType ListingType { get; set; } //e.g. sale, lease
    public int Price { get; set; } 
    public required string[] Features { get; set; }
    public required string TenWordSummary { get; set; }
};



[JsonConverter(typeof(JsonStringEnumConverter))]
enum CarListingType
{
    Sale,
    Lease
}
*/
#endregion

#region  ChatpApp

/*
List<ChatMessage> chatHistory = new()
{
    new ChatMessage(ChatRole.System, """        
        You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
        You introduce yourself when first saying hello.
        When helping people out, you always ask them for this information
        to inform the hiking recommendation you provide:
        
        1. The location where they would like to hike
        2. What hiking intensity they are looking for
        
        You will then provide three suggestions for nearby hikes that vary in length
        after you get that information. You will also share an interesting fact about
        the local nature on the hikes when making a recommendation. At the end of your
        response, ask if there is anything else you can help with.
    """),
};

while(true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt ?? string.Empty));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response="";
    await foreach(var item in client.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}
*/
#endregion

