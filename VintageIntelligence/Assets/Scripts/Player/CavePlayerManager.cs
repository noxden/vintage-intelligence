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
    [SerializeField] private List<GameObject> PossessedObjects = new();

    private GameObject _lastPossessed;

    // Transfers the position of the cave player over the network
    public void SetPossessingObjectForOthers(int objectIndex, GameObject possessingObject)
    {
        _lastPossessed?.SetActive(true);
        possessingObject.SetActive(false);
        _lastPossessed = possessingObject;

        photonView.RPC("SetPossessingObject", RpcTarget.Others, objectIndex);
    }

    // Turns on an indicator for the cave player position
    [PunRPC]
    public void SetPossessingObject(int objectIndex)
    {
        // Possessable Objects raw (newest off, rest on)
        foreach(GameObject obj in PosessableObjects) 
            obj.SetActive(true);
        
        PosessableObjects[objectIndex]?.SetActive(false);

        // Possessed Objects cave player indicator (newest on, rest off)
        foreach (GameObject obj in PossessedObjects) 
            obj.SetActive(false);

        PossessedObjects[objectIndex]?.SetActive(true);
    }

}
