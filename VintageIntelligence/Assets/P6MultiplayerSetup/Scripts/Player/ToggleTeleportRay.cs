using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

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
