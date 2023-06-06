using Photon.Pun;

using UnityEngine;

public class TransferMessage : MonoBehaviourPun
{
    private string myString = "Hello, World!";

    private void Start()
    {
        SpeechManager.OnNewRecognizedText += SendString;
    }

    public void SendString(string message)
    {
        photonView.RPC("ReceiveString", RpcTarget.Others, message);
    }

    [PunRPC]
    private void ReceiveString(string receivedString)
    {
        SpeechManager.StartReadMessage(receivedString);
        Debug.Log("Received string: " + receivedString);
    }
}
