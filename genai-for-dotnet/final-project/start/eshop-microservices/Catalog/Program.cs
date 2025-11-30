
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<CatalogDbContext>(connectionName: "catalogdb");

builder.Services.AddScoped<ProductService>();


#region AI Client Configuration

    // Add AI Chat Client
    var credential = new ApiKeyCredential(builder.Configuration["GitHubModels:Token"]);
    var options = new OpenAIClientOptions()
    {
        Endpoint = new Uri("https://models.github.ai/inference")
    };

    IChatClient chatClient = 
        new OpenAIClient(credential, options).GetChatClient("openai/gpt-5-mini").AsIChatClient();

    builder.Services.AddChatClient(chatClient);

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseMigration();

app.MapProductEndpoints();

app.Run();
