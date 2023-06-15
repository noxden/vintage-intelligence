// Created by Krista Plagemann //
// Manages the status of the cave player, as well as the position for others.

using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CavePlayerManager : MonoBehaviourPunCallbacks
{

    #region Singleton

    public static CavePlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #endregion

    [SerializeField] private List<GameObject> PosessableObjects = new();

    // Transfers the position of the cave player over the network
    public void SetPossessingObjectForOthers(int objectIndex)
    {
        photonView.RPC("SetPossessingObject", RpcTarget.Others, objectIndex);
    }

    // Turns on an indicator for the cave player position
    [PunRPC]
    public void SetPossessingObject(int objectIndex)
    {
        foreach(GameObject obj in PosessableObjects) 
            obj.SetActive(false);
        
        PosessableObjects[objectIndex]?.SetActive(true);
    }

}
