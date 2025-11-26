using System.ClientModel;
using System.Numerics.Tensors;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var credential = new ApiKeyCredential(configuration["GitHubModels:Token"] ?? throw new InvalidOperationException("GitHub Models token not found in configuration."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};
var model="openai/gpt-5-mini"; //"deepseek/DeepSeek-V3-0324"
IChatClient client = new OpenAIClient(credential, options)
    .GetChatClient(model).AsIChatClient();

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = 
    new OpenAIClient(credential, options).GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

/*
// 1: Generate a single embedding
var embedding = await embeddingGenerator.GenerateVectorAsync("Hello, world!");
foreach (var value in embedding.Span)
{
    Console.WriteLine("{0:0.00}, ",value);
}
*/

// 2: Compare multiple embeddings using Cosine Similarity

var catVector = await embeddingGenerator.GenerateVectorAsync("cat");
var dogVector = await embeddingGenerator.GenerateVectorAsync("dog");
var puppyVector = await embeddingGenerator.GenerateVectorAsync("puppy");
var kittenVector = await embeddingGenerator.GenerateVectorAsync("kitten");

Console.WriteLine($"Cosine Similarity between cat and dog: {TensorPrimitives.CosineSimilarity(catVector.Span, dogVector.Span):F2}");
Console.WriteLine($"Cosine Similarity between dog and puppy: {TensorPrimitives.CosineSimilarity(dogVector.Span, puppyVector.Span):F2}");
Console.WriteLine($"Cosine Similarity between cat and kitten: {TensorPrimitives.CosineSimilarity(catVector.Span, kittenVector.Span):F2}");
Console.WriteLine($"Cosine Similarity between dog and kitten: {TensorPrimitives.CosineSimilarity(dogVector.Span, kittenVector.Span):F2}");
Console.WriteLine($"Cosine Similarity between puppy and kitten: {TensorPrimitives.CosineSimilarity(puppyVector.Span, kittenVector.Span):F2}");