// Created by Krista Plagemann //
// Toggles the visuals of the teleport ray only when pressing the trigger.

using UnityEngine;
using UnityEngine.InputSystem;


public class ToggleTeleportRay : MonoBehaviour
{
    [SerializeField] private GameObject _RTeleportRay;
    [SerializeField] private GameObject _LTeleportRay;

    [SerializeField] private InputActionProperty _RightActive;
    [SerializeField] private InputActionProperty _LeftActive;

    private void Update()
    {
        _RTeleportRay.SetActive(_RightActive.action.ReadValue<float>() > 0.1f);
        _LTeleportRay.SetActive(_LeftActive.action.ReadValue<float>() > 0.1f);
    }
}
