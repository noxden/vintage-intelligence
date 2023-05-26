using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using OpenAI;
using UnityEngine;

class SpeechManager : MonoBehaviour
{
    /////////////////// SST and TTS
       
    // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
    static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    static SpeechConfig speechConfig;
    static AudioConfig audioConfig;

    /////////////////// ChatGPT

    private OpenAIApi openai = new OpenAIApi("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");

    private string returnText;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = "Can you please convert the following text into a shakespearean dialect without adding any other response? " +
        " ";

    public async void Start()
    {
        speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
        speechConfig.SpeechRecognitionLanguage = "en-US";
        speechConfig.SpeechSynthesisVoiceName = "en-AU-DarrenNeural";

        audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        using var speechSynthesizer = new SpeechSynthesizer(speechConfig, audioConfig);
        using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        Debug.Log("Speak into your microphone.");

        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        if (OutputSpeechRecognitionResult(speechRecognitionResult))
        {
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = GetChatGPTMessage(speechRecognitionResult.Text)
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var returnMessage = completionResponse.Choices[0].Message;
                returnMessage.Content = returnMessage.Content.Trim();

                returnText = returnMessage.Content.ToString();
                Debug.Log(returnText);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
                returnText = "";
            }

            if (returnText.Length != 0)
            {
                using (speechSynthesizer)
                {
                    var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(returnText);
                    OutputSpeechSynthesisResult(speechSynthesisResult, returnText);
                }
            }
        }
    }

    #region SST and TTS


    static bool OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                Debug.Log($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                return true;
            case ResultReason.NoMatch:
                Debug.Log($"NOMATCH: Speech could not be recognized.");
                return false;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                return false;
        }
        return false;
    }

    static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Debug.Log($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }

    #endregion

    #region ChatGPT


    private List<ChatMessage> GetChatGPTMessage(string message)
    {
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = prompt + message
        };

        messages.Clear();
        messages.Add(newMessage);

        return messages;
    }

    #endregion
}