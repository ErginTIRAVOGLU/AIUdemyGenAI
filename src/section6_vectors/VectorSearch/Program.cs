using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using VectorSearch;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var credential = new ApiKeyCredential(configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("GitHub Models token not found in configuration."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IEmbeddingGenerator<string, Embedding<float>> generator =
    new OpenAIClient(credential, options)
        .GetEmbeddingClient("openai/text-embedding-3-small")
        .AsIEmbeddingGenerator();

// Create an embedding generator (text-embedding-3-small is an example)

var vectorStore = new InMemoryVectorStore();

var moviesStore = vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    // generate the embedding vectore for the movie description
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);

    // add the overall movie to the in-memory vector store's movie collection
    await moviesStore.UpsertAsync(movie);
}


// 1-Embed the user's query
// 2-Vectorized search
// 3-Returns the records

// generate the embedding vector for the user's prompt
var query = "I want to see family friendly movie";
//var query = "A science fiction movie about space travel";
//var query = "A science fiction movie about space wars and intergalactic battles";
var queryEmbedding = await generator.GenerateVectorAsync(query);


// search the knowledge store based on the user's prompt
var searchResults = moviesStore.SearchAsync(queryEmbedding, top:2);

//see the results just so we know what they look like
await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine("--------------------------------------------------");
}



