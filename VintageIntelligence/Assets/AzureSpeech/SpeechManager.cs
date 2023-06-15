// Author: Krista Plagemann //
// Integrates Azure speech recognition and speech synthesis as well as ChatGPT.

// 1. Use "StartSpeechRecording()" and "StopSpeechRecording()" to start and stop a voice recording.
// 2. Subscribe to OnNewRecognizedText to receive the string output of the recognized speech.
// 3. Use "StartReadMessage(string message)" to convert with ChatGPT and read out the ChatGPT result with Azure speech synthesis.

// Additional notes:
// - Set "_speechRecognitionLanguage" to whatever language you are speaking when recording.
// - Use "SetChatGPTPrompt(string prompt)" to adjust the "_prompt" to change what ChatGPT outputs (or do it in this script directly).
// - Subscribe to "OnNewSpokenText" to receive the ChatGPT result/the string that is being read out.
// - In "GetStyledVoiceString(string message)" you can change the spoken language (e.g. <voice name='de-DE-ConradNeural'>)
// and style of voice (e.g. < mstts:express -as style = ""cheerful"">).
// - The keys are stored in a separate unpublished script called AIData. Set the AzureKey and AzureRegion as you would in your environment variables
// and ChatGPTKey as you would in your auth.json file. Use a plain string with only the key or region in it.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using OpenAI;
using UnityEngine;


public static class SpeechManager
{
    /////////////////// STT and TTS

    private static SpeechConfig _speechConfig;
    private static AudioConfig _audioConfig;
    private static SpeechRecognitionResult _speechRecognitionResult;
    private static string _speechRecognitionLanguage = "en-US"; // Changes the language azure is trying to recognize

    public static bool FinishedRecording = false;
    public static TaskCompletionSource<int> StopRecognition;

    /// <summary>
    /// When we recorded a new voice block and converted it to text. Outputs string of recorded text.
    /// </summary>
    public static event Action<string> OnNewRecognizedText;

    /// <summary>
    /// When a message is converted by ChatGPT. Outputs the text that is being read out.
    /// </summary>
    public static event Action<string> OnNewSpokenText;

    /////////////////// ChatGPT

    private static OpenAIApi _openai = new OpenAIApi();

    private static string _returnText;
    private static List<ChatMessage> _messages = new List<ChatMessage>();
    private static string _prompt = "Can you please convert the following text into a Shakespearean dialect without adding any other response or reapeating this question? " +
        " ";

    #region RecordingSpeech

    /// <summary>
    /// Start recording what the player says from now on.
    /// </summary>
    public static async void StartSpeechRecording()
    {
        FinishedRecording = false;
        await RunSpeechRecording();
    }

    /// <summary>
    /// Stop recording and let azure analyze what the player said.
    /// </summary>
    public static void StopSpeechRecording()
    {
        FinishedRecording = true;
        Debug.Log("Recording Stopped");
    }

    public static async Task RunSpeechRecording()
    {
        _speechConfig = SpeechConfig.FromSubscription(AIData.AzureKey, AIData.AzureRegion);
        _speechConfig.SpeechRecognitionLanguage = _speechRecognitionLanguage;
        _audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        using var speechSynthesizer = new SpeechSynthesizer(_speechConfig, _audioConfig);
        using var speechRecognizer = new SpeechRecognizer(_speechConfig, _audioConfig);
        StopRecognition = new TaskCompletionSource<int>();

        #region Subscribing to SpeechRecognizingEvents

        speechRecognizer.Recognized += SpeechRecognized;
        speechRecognizer.Canceled += SpeechCanceled;
        speechRecognizer.SessionStopped += SpeechedStopped;

        #endregion


        //////////// SPEECH TO TEXT //////////////
        
        await speechRecognizer.StartContinuousRecognitionAsync(); // Starts recording what the player is saying

        while (!FinishedRecording) { await Task.Yield(); }  // Waits until we declare the recording finished.
        
        await speechRecognizer.StopContinuousRecognitionAsync();    // Stops recording

        // Waits for completion of the stopping
        Task.WaitAny(new[] { StopRecognition.Task });

        if (_speechRecognitionResult == null)    // just to make sure we have a result. If not, we cancel the whole thing to avoid errors
            return;

        OutputSpeechRecognitionResult(_speechRecognitionResult); // Validates the output and fires OnNewRecognizedText with the string received

        // Finished with firing off the event OnNewRecognizedText. Subscribe to it and save the string somewhere to use it further.
    }

    #endregion

    #region Converting in ChatGPT and reading it out

    private static string _messageToRead = "";

    /// <summary>
    /// Call this to start converting the message given in ChatGPt and reading it out for this user.
    /// </summary>
    /// <param name="message">Message that will be sent to ChatGPT along with a prompt defined in the script.</param>   
    public static async void StartReadMessage(string message)
    {
        _messageToRead = message;
        Debug.LogWarning("Reading it out now.");
        await ReadMessageAsync();
    }

    /// <summary>
    /// Sets the prompt for chatgpt. Make sure to write it in a way that removes any unnecessary filler explanations by chatGPT.
    /// </summary>
    /// <param name="prompt">The prompt will have the recognized text attach at the end.</param>
    public static void SetChatGPTPrompt(string prompt) { _prompt = prompt; }

    public static async Task ReadMessageAsync()
    {
        _speechConfig = SpeechConfig.FromSubscription(AIData.AzureKey, AIData.AzureRegion);
        _speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);

        using var speechSynthesizer = new SpeechSynthesizer(_speechConfig, _audioConfig);

        if (_messageToRead.Length <= 0) // makes sure the recognizing before was successful and we have a text
            return;

        //////////// CHAT GPT //////////////
        _openai = new OpenAIApi(AIData.ChatGPTKey);

        // Sends the prompt and recognized text to ChatGPT and waits for a response
        var completionResponse = await _openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo-0301",
            Messages = GetChatGPTMessage(_messageToRead)
        });

        // Extracts the first choice of responses from the List we get back.
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
        if (_returnText.Length != 0)    // only if the text contains something
        {
            OnNewSpokenText?.Invoke(_returnText);   // Fires off an event with the converted ChatGPT message
            using (speechSynthesizer)
            {
                // Reads out the text we got from ChatGPT
                var speechSynthesisResult = await speechSynthesizer.SpeakSsmlAsync(GetStyledVoiceString(_returnText));
                OutputSpeechSynthesisResult(speechSynthesisResult, _returnText); // literally just Debug.Logs :D
            }
        }
    }

    #endregion

    #region STT and TTS EventListeners and Output validators

    // These are just reactions to what happens in the speech recognizing and speaking process. You can get rid of all the Debug.Logs if you don't want to clutter your console.
    // Some of them contain important steps though, so make sure to only delete Debug.Logs :)
    private static void SpeechRecognized(object sender, SpeechRecognitionEventArgs eventArgs)
    {
        if (eventArgs.Result.Reason == ResultReason.RecognizedSpeech)
        {
            Debug.Log($"RECOGNIZED: Text={eventArgs.Result.Text}");
        }
        else if (eventArgs.Result.Reason == ResultReason.NoMatch)
        {
            Debug.Log($"NOMATCH: Speech could not be recognized.");
        }

        _speechRecognitionResult = eventArgs.Result;    // saves the recognition result

    }
    private static void SpeechCanceled(object sender, SpeechRecognitionCanceledEventArgs eventArgs)
    {
        Debug.Log($"CANCELED: Reason={eventArgs.Reason}");

        if (eventArgs.Reason == CancellationReason.Error)
        {
            Debug.Log($"CANCELED: ErrorCode={eventArgs.ErrorCode}");
            Debug.Log($"CANCELED: ErrorDetails={eventArgs.ErrorDetails}");
            Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
        }

        StopRecognition.TrySetResult(0);    // Makes sure the stopping is registered and continues the Task that waits for this.
    }
    private static void SpeechedStopped(object sender, SessionEventArgs eventArgs)
    {
        Debug.Log("\n    Session stopped event.");
        StopRecognition.TrySetResult(0); // Makes sure the stopping is registered and continues the Task that waits for this.
    }


    static bool OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                {
                    Debug.Log($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                    OnNewRecognizedText?.Invoke(speechRecognitionResult.Text);          // Fires off an event with the recorded text as string
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

    // Returns the format that ChatGPT wants lol
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

    #endregion

    #region SpeechOutput


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