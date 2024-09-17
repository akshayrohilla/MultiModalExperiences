#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.AudioToText;

namespace MultiModalCopilot;

public class AudiotoText()
{
    public static async Task AudiotoTextOpenAI()
    {
        // Create kernel builder
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        // Audio to Text service to convert audio to text
        kernelBuilder.AddAzureOpenAIAudioToText("whisper", Environment.GetEnvironmentVariable("AZURE_WHISPER_ENDPOINT")!, Environment.GetEnvironmentVariable("AZURE_WHISPER_APIKEY")!);

        // Create kernel
        var kernel = kernelBuilder.Build();

        var chat = kernel.GetRequiredService<IAudioToTextService>();

        while (true)
        {
            Console.Write("Where is the audio file located? : ");
            Console.ForegroundColor = ConsoleColor.Green;
            string audioFilePath = Console.ReadLine() ?? string.Empty;
            Console.ResetColor();

            // Read the audio file
            var audioFileStream = File.OpenRead(audioFilePath);
            var audioFileBinaryData = await BinaryData.FromStreamAsync(audioFileStream!);
            AudioContent audioContent = new(audioFileBinaryData, mimeType: null);

            var textContent = await chat.GetTextContentAsync(audioContent);

            Console.Write("\n Audio transcription : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(textContent.Text);
            Console.ResetColor();

            Console.Write("\n Do you want to transcribe another audio? (Y/N) : ");
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