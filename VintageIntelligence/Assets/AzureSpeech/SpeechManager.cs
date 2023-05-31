using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using OpenAI;
using UnityEngine;


public static class SpeechManager
{
    /////////////////// SST and TTS

    // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
    static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    static SpeechConfig speechConfig;
    static AudioConfig audioConfig;

    /////////////////// ChatGPT

    private static OpenAIApi openai = new OpenAIApi();

    private static string _returnText;
    private static List<ChatMessage> _messages = new List<ChatMessage>();
    private static string _prompt = "Can you please convert the following text into a Shakespearean dialect without adding any other response or reapeating this question? " +
        " ";
    private static SpeechRecognitionResult speechRecognitionResult;

    public static bool FinishedRecording = false;
    public static TaskCompletionSource<int> stopRecognition;

    public static event Action<string> OnNewRecognizedText;
    public static event Action<string> OnNewSpokenText;

    public static async void StartSpeechRecording()
    {
        FinishedRecording = false;
        await RunSpeechRecording();
    }

    public static void StopSpeechRecording()
    {
        FinishedRecording = true;
        Debug.Log("recording Stopped");
        // Make the following call at some point to stop recognition:
        //await _speechRecognizer.StopContinuousRecognitionAsync();
    }

    public static async Task RunSpeechRecording()
    {
        //////////// SPEECH TO TEXT //////////////
        speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
        speechConfig.SpeechRecognitionLanguage = "de-DE";
        speechConfig.SpeechSynthesisVoiceName = "en-US-AriaNeural";

        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);

        audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        using var speechSynthesizer = new SpeechSynthesizer(speechConfig, audioConfig);
        using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        stopRecognition = new TaskCompletionSource<int>();

        #region SpeechRecognizingEvents

        speechRecognizer.Recognized += SpeechRecognized;
        speechRecognizer.Canceled += SpeechCanceled;
        speechRecognizer.SessionStopped += SpeechedStopped;

        #endregion

        await speechRecognizer.StartContinuousRecognitionAsync();
 
        while (!FinishedRecording) { await Task.Yield(); }
        
        await speechRecognizer.StopContinuousRecognitionAsync();

        // Waits for completion. Use Task.WaitAny to keep the task rooted.
        Task.WaitAny(new[] { stopRecognition.Task });

        if (speechRecognitionResult == null)
            return;

        //////////// CHAT GPT //////////////
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

                _returnText = returnMessage.Content.ToString();
                Debug.Log(_returnText);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
                _returnText = "";
            }
            //////////// TEXT TO SPEECH //////////////
            if (_returnText.Length != 0)
            {
                OnNewSpokenText?.Invoke(_returnText);
                using (speechSynthesizer)
                {
                    //var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(_returnText);

                    var speechSynthesisResult = await speechSynthesizer.SpeakSsmlAsync(GetStyledVoiceString(_returnText));
                    OutputSpeechSynthesisResult(speechSynthesisResult, _returnText);
                }
            }
        }
    }

    #region SST and TTS

    private static void SpeechRecognized(object sender, SpeechRecognitionEventArgs eventArgs)
    {
        if (eventArgs.Result.Reason == ResultReason.RecognizedSpeech)
        {
            Console.WriteLine($"RECOGNIZED: Text={eventArgs.Result.Text}");
        }
        else if (eventArgs.Result.Reason == ResultReason.NoMatch)
        {
            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
        }
        
        speechRecognitionResult = eventArgs.Result;

    }
    private static void SpeechCanceled(object sender, SpeechRecognitionCanceledEventArgs eventArgs)
    {

        Console.WriteLine($"CANCELED: Reason={eventArgs.Reason}");

        if (eventArgs.Reason == CancellationReason.Error)
        {
            Console.WriteLine($"CANCELED: ErrorCode={eventArgs.ErrorCode}");
            Console.WriteLine($"CANCELED: ErrorDetails={eventArgs.ErrorDetails}");
            Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
        }

        stopRecognition.TrySetResult(0);
    }
    private static void SpeechedStopped(object sender, SessionEventArgs eventArgs)
    {
        Console.WriteLine("\n    Session stopped event.");
        stopRecognition.TrySetResult(0);
    }


    static bool OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                {
                    Debug.Log($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                    OnNewRecognizedText?.Invoke(speechRecognitionResult.Text);
                }
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


    private static List<ChatMessage> GetChatGPTMessage(string message)
    {
        
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = _prompt + message
        };

        _messages.Clear();
        _messages.Add(newMessage);

        return _messages;
    }

    private static string GetStyledVoiceString(string message)
    {
        var ssml = @$"<speak version='1.0' xml:lang='de-DE' xmlns='http://www.w3.org/2001/10/synthesis' xmlns:mstts='http://www.w3.org/2001/mstts'>
                        <voice name='de-DE-ConradNeural'>
                            <mstts:express-as style=""cheerful"">
                {message}
                </mstts:express-as>
            </voice>
        </speak>";

        return ssml;
    }
    #endregion
}