// Created 31.05.2023 by Krista Plagemann //
// Manages toggling input on and off. //


using System;
using UnityEngine;

public class InputStatusManager : MonoBehaviour
{
    public static InputStatusManager Instance { get; private set; }
    private void Awake() => Instance = this;
    
    public event Action<bool> OnSpeechInputToggle = delegate { };

    public void ToggleSpeechInput(bool state)
    {
        OnSpeechInputToggle?.Invoke(state);
    }
}
