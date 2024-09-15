using OpenCvSharp;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MultiModalCopilot;

public class VideoToText()
{
    public static async Task VideotoTextOpenAI()
    {

        Console.Write("Where is the video file located? : ");
        Console.ForegroundColor = ConsoleColor.Green;
        string videoFilePath = Console.ReadLine() ?? string.Empty;
        Console.ResetColor();

        // Create a directory to store the frames
        string outputDirectory = "Frames/";
        Directory.CreateDirectory(outputDirectory);

        using (var capture = new VideoCapture(videoFilePath))
        {
            int totalFrames = (int)capture.FrameCount;
            double fps = capture.Fps;
            double duration = totalFrames / fps;
            int numberOfFrames = (int)duration; // One frame per second
            int frameInterval = totalFrames / numberOfFrames;

            for (int i = 0; i < numberOfFrames; i++)
            {
                int frameNumber = i * frameInterval;
                capture.Set(VideoCaptureProperties.PosFrames, frameNumber);
                using (var frame = new Mat())
                {
                    capture.Read(frame);
                    if (frame.Empty())
                        break;

                    string frameFilePath = $"{outputDirectory}frame_{i}.jpg";
                    Cv2.ImWrite(frameFilePath, frame);
                    Console.WriteLine($"Extracted frame {i} to {frameFilePath}");
                }
            }

            await VideoSummarizer(numberOfFrames);
        }
    }

    public static async Task VideoSummarizer(int numberOfFrames)
    {
        // Create kernel builder
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        // Chat completion service.
        kernelBuilder.AddAzureOpenAIChatCompletion("gpt4o", Environment.GetEnvironmentVariable("AZURE_OAI_ENDPOINT")!, Environment.GetEnvironmentVariable("AZURE_OAI_APIKEY")!);

        // Create kernel
        var kernel = kernelBuilder.Build();

        var chat = kernel.GetRequiredService<IChatCompletionService>();
        var history = new ChatHistory("You are a useful assistant analyzing a video frame by frame. Ask questions about the video to get answers. Answer concisely and directly.");

        while (true)
        {
            //read the image from a path
            for (int i = 0; i < numberOfFrames; i++)
            {
                var imageData = File.ReadAllBytes($"Frames/frame_{i}.jpg");
                ImageContent imageContent = new(new BinaryData(imageData), "image/jpg");

                var collectionItems = new ChatMessageContentItemCollection
                {
                    imageContent
                };

                history.AddUserMessage(collectionItems);
            }

            Console.Write("\nWhat do you want to ask about the video? : ");
            Console.ForegroundColor = ConsoleColor.Green;
            var ask = Console.ReadLine() ?? string.Empty;
            Console.ResetColor();

            history.AddUserMessage(ask);

            var result = await chat.GetChatMessageContentsAsync(history);
            
            Console.Write("\nCopilot : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(result[^1].Content);
            Console.ResetColor();

            Console.Write("\nDo you want to ask another question about the video? (Y/N) : ");
            Console.ForegroundColor = ConsoleColor.Green;
            var response = Console.ReadLine();
            Console.ResetColor();

            if (string.Equals(response, "N", StringComparison.OrdinalIgnoreCase))
            {
                //delete the frames
                Directory.Delete("Frames", true);
                break;
            }
        }
    }
}