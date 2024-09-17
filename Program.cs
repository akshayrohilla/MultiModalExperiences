using MultiModalCopilot;

Console.WriteLine("Select an option:");
Console.WriteLine("1. Image to Text");
Console.WriteLine("2. Video to Text");
Console.WriteLine("3. Audio to Text");
Console.WriteLine("4. Speech Translation");

string option = Console.ReadLine() ?? string.Empty;
Console.Clear();

switch (option)
{
    case "1":
        Console.WriteLine("Image to Text option selected");
        Console.WriteLine("--------------------------------- \n");
        await ImagetoText.ImagetoTextOpenAI();
        break;
    case "2":
        Console.WriteLine("Video to Text option selected");
        Console.WriteLine("--------------------------------- \n");
        await VideoToText.VideotoTextOpenAI();
        break;
    case "3":
        Console.WriteLine("Audio to Text option selected");
        Console.WriteLine("--------------------------------- \n");
        await AudiotoText.AudiotoTextOpenAI();
        break;
    case "4":
        Console.WriteLine("Speech Translation option selected");
        Console.WriteLine("--------------------------------- \n");
        await SpeechTranslation.SpeechTranslationAI();
        break;
    default:
        Console.WriteLine("Invalid option selected");
        break;
}
