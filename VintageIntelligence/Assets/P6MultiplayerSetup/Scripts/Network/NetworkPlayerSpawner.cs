// Created on base of youtube tutorial by Valem //
// Instantiates a prefab as the player avatar when we join a room and destroys it again when we leave a room. //


using Photon.Pun;
using UnityEngine;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject CavePlayer;
    public GameObject HMDPlayer;

    private GameObject _spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Check for displays to determine which player this is
        if (Display.displays.Length > 6)
            SetupCavePlayer();
        else
            SetupHMDPlayer();

        // When we join a room, we spawn an Avatar for ourselves(the NetworkPlayer). This will be visible to others then since we instantiate it in the network.
        _spawnedPlayerPrefab = PhotonNetwork.Instantiate("NetworkPlayer", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // When we leave the room again, we also destroy our avatar on the network
        PhotonNetwork.Destroy(_spawnedPlayerPrefab);
    }

    private void SetupCavePlayer()
    {
        CavePlayer.SetActive(true);
        Transform caveOrigin = CavePlayer.transform.GetChild(0);
        caveOrigin.parent = null;
    }

    private void SetupHMDPlayer()
    {
        HMDPlayer.SetActive(true);
        Transform hmdOrigin = HMDPlayer.transform.GetChild(0);
        hmdOrigin.parent = null;
    }
}
