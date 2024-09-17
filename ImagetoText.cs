using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

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

        var gpt_svc = kernel.GetRequiredService<IChatCompletionService>();

        while (true)
        {
            var chat = new ChatHistory("You are a useful assistant that helps to answer questions about an image in a single sentence.");
            
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

            chat.AddUserMessage(collectionItems);

            var result = await gpt_svc.GetChatMessageContentAsync(chat);

            Console.WriteLine("\n Image description : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            
            Console.WriteLine(result.Content);

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