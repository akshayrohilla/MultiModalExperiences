#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001
using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace MultiModalCopilot;

public class ImagetoText()
{

    public static async Task ImagetoTextOpenAI()
    {
        // Create kernel builder
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        // Chat completion service.
        kernelBuilder.AddAzureOpenAIChatCompletion("gpt4o", Environment.GetEnvironmentVariable("AZURE_OAI_ENDPOINT")!, Environment.GetEnvironmentVariable("AZURE_OAI_APIKEY")!);

        // Create kernel
        var kernel = kernelBuilder.Build();

        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory("You are a useful assistant that helps to describe an image. Ask questions about the image to get answers.");

        while (true)
        {
            var imageContent = new ImageContent();

            Console.Write("Enter the image URL : ");

            Console.ForegroundColor = ConsoleColor.Green;
            var image = Console.ReadLine() ?? string.Empty;
            Console.ResetColor();
            // use remote image
            imageContent.Uri = new Uri(image);

            Console.Write("\n What do you want to ask about the image? : ");
            Console.ForegroundColor = ConsoleColor.Green;
            var ask = Console.ReadLine();
            Console.ResetColor();
            var collectionItems = new ChatMessageContentItemCollection
            {
                new TextContent(ask),
                imageContent
            };

            history.AddUserMessage(collectionItems);

            var result = await chat.GetChatMessageContentsAsync(history);
            Console.WriteLine("\n Image description : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(result[^1].Content);
            Console.ResetColor();

            Console.Write("\n Do you want to describe another image? (Y/N) : ");
            Console.ForegroundColor = ConsoleColor.Green;
            var response = Console.ReadLine();
            Console.ResetColor();

            if (string.Equals(response, "N", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }
    }
}