using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChatGPTIntegration : MonoBehaviour
{
    private OpenAIApi openai = new OpenAIApi();

    private List<ChatMessage> messages = new List<ChatMessage>();
    private string prompt = "Convert this text into old english language(do not say anything other than the text.): ";
    private string returnText;

    private async void SendReply(string message)
    {
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = message
        };

        string textToSend = $"{prompt} {newMessage.Content}";

        messages.Clear();
        messages.Add(newMessage);

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo-0301",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var returnMessage = completionResponse.Choices[0].Message;
            returnMessage.Content = returnMessage.Content.Trim();

            returnText = returnMessage.Content.ToString();
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }
    }
}
