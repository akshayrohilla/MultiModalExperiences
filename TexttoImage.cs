#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextToImage;

namespace MultiModalCopilot;

public class TexttoImage()
{

    public static async Task TexttoImageOpenAI()
    {
        // Create kernel builder
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        // Text to Image service to generate image from text
        kernelBuilder.AddAzureOpenAITextToImage("dall-e-3", Environment.GetEnvironmentVariable("AZURE_DALLE_ENDPOINT")!, Environment.GetEnvironmentVariable("AZURE_DALLE_APIKEY")!);

        // Create kernel
        var kernel = kernelBuilder.Build();

        var chat = kernel.GetRequiredService<ITextToImageService>();

        var history = new ChatHistory();

        while(true)
        {
            Console.Write("Provide the description of the Image : ");
            Console.ForegroundColor = ConsoleColor.Green;
            var ask = Console.ReadLine() ?? string.Empty;
            Console.ResetColor();
            history.AddUserMessage(ask);

            var image = await chat.GenerateImageAsync(ask, 1024, 1024);
            Console.Write("\n Image generated : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(image);
            Console.ResetColor();

            Console.Write("\n Do you want to generate another image? (Y/N) : ");
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