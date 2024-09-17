using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;

namespace MultiModalCopilot;

public class SpeechTranslation()
{
    public static async Task SpeechTranslationAI()
    {
        var speechTranslationConfig =
            SpeechTranslationConfig.FromSubscription(Environment.GetEnvironmentVariable("AZURE_SPEECH_APIKEY")!, Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION")!);

        var fromLanguage = "en-Us";
        var toLanguage = "fr";
        speechTranslationConfig.SpeechRecognitionLanguage = fromLanguage;
        speechTranslationConfig.AddTargetLanguage(toLanguage);

        speechTranslationConfig.VoiceName = "de-DE-Hedda";

        using var translationRecognizer = new TranslationRecognizer(speechTranslationConfig);

        translationRecognizer.Synthesizing += (_, e) =>
        {
            var audio = e.Result.GetAudio();
            Console.WriteLine($"Audio synthesized: {audio.Length:#,0} byte(s) {(audio.Length == 0 ? "(Complete)" : "")}");

            if (audio.Length > 0)
            {
                File.WriteAllBytes("YourAudioFile.wav", audio);
            }
        };

        Console.Write($"Say something in '{fromLanguage}' and ");
        Console.WriteLine($"we'll translate into '{toLanguage}'.\n");

        var result = await translationRecognizer.RecognizeOnceAsync();
        if (result.Reason == ResultReason.TranslatedSpeech)
        {
            Console.WriteLine($"Recognized: \"{result.Text}\"");
            Console.WriteLine($"Translated into '{toLanguage}': {result.Translations[toLanguage]}");
        }
    }
}