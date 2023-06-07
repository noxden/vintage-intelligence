// Created on base of youtube tutorial by Valem //
// Initializes the connection to the server and gives status updates via Debug.Log. Also creates a room "Room 1" once connected or joins said room if exciting already. //


using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        ConnectToServer();
    }

    // Connect to the Photon server on start using the settings you set in the inspector when setting up the server.
    private void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Trying to connect to server...");
    }

    // We are getting the following functions from the MonoBehaviourPunCallbacks that we derive this class form
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server.");
        base.OnConnectedToMaster();

        // We make a new set of room options for the room we want to open. You can assign these with a 
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 10,    // hover over names to see explanations
            IsVisible = true,  
            IsOpen = true
        };
        // When we connect to the server we create or join "Room 1".
        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
        base.OnJoinedRoom();

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Someone joined a room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }


}