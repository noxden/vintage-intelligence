// Created by Max von Trümbach and Krista Plagemann
// Handles closing the UI elements that contain a small controller tutorial.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;


public class CloseUIElements : MonoBehaviour
{
    [SerializeField] private CP_Raycast _CavePlayerRaycast;

    [SerializeField] private GameObject objectToHide1;
    [SerializeField] private GameObject objectToHide2;

    private InputDevice _rightHand;
    [SerializeField] private InputDeviceCharacteristics controllerToUse;
    [SerializeField] private float TriggerStartThreshold = 0.6f;
    [SerializeField] private float GripStartThreshold = 0.6f;
    [SerializeField] private float triggerDelay = 1.0f;

    private async void Start()
    {
        TryInitializeController();
        await WaitForGripButton();
    }

    private void TryInitializeController()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerToUse, devices);
        if (devices.Count > 0)
            _rightHand = devices[0];
        
        //Debug.Log("Controller for UI Stuff: "+_rightHand.name + _rightHand.characteristics);
    }

    private bool _triggerPressed = false;
    private bool _gripPressed = false;

    private async Task WaitForGripButton()
    {
        while (!_gripPressed)
        {
            if (!_rightHand.isValid)
            {
                TryInitializeController();
            }

            if (_rightHand.TryGetFeatureValue(CommonUsages.grip, out var rGripPressed))
            {
                if (rGripPressed >= GripStartThreshold)
                {

                    // Toggle the visibility of the first object
                    objectToHide1.SetActive(false);
                    objectToHide2.SetActive(true);
                    _gripPressed = true;
                    await WaitForTriggerButton();
                }
            }
            await Task.Yield();
        }
    }

    private async Task WaitForTriggerButton()
    {
        while (!_triggerPressed)
        {
            if (!_rightHand.isValid)
                TryInitializeController();


            if (_rightHand.TryGetFeatureValue(CommonUsages.trigger, out var rTriggerPressed))
            {
                if (rTriggerPressed >= GripStartThreshold)
                {
                    // Toggle the visibility of the first object
                    _triggerPressed = true;
                }
            }
            await Task.Yield();
        }
        if(_triggerPressed)
        {
            Invoke("EnableNormalInteractions", 1f);
            objectToHide2.SetActive(false);
        }
    }

    private void EnableNormalInteractions()
    {
        if (PuzzleHandler.Instance._ThisPlayer == PlayerType.CavePlayer) // enable normal interactions for cave player
            _CavePlayerRaycast.enabled = true;
        gameObject.SetActive(false);
    }
}