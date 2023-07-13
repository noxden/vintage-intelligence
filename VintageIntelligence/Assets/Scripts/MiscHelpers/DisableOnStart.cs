// Created by Krista Plagemann//
// Just to fix photonview logic


using UnityEngine;

public class DisableOnStart : MonoBehaviour
{

    void Start()
    {
        gameObject.SetActive(false);
    }
}
