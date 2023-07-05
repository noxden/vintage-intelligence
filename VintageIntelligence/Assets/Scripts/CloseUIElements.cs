using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CloseUIElements : MonoBehaviour
{
    public GameObject objectToHide1;
    public GameObject objectToHide2;
    public GameObject objectToHide3;

    private bool isHidden1;
    private bool isHidden2;

    private InputDevice _leftHand;
    [SerializeField] private InputDeviceCharacteristics controllerToUse;
    [SerializeField] private float TriggerStartThreshold = 0.6f;
    [SerializeField] private float triggerDelay = 1.0f;

    private float triggerTimer;

    private void Start()
    {
        TryInitializeController();
    }

    private void TryInitializeController()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerToUse, devices);
        if (devices.Count > 0)
            _leftHand = devices[0];
        
        Debug.Log("Controller for UI Stuff: "+_leftHand.name + _leftHand.characteristics);
    }

    private void Update()
    {
        Debug.Log(_leftHand.TryGetFeatureValue(CommonUsages.trigger, out float data));
        if (_leftHand.TryGetFeatureValue(CommonUsages.trigger, out var rTriggerPressed))
        {
            if (rTriggerPressed >= TriggerStartThreshold)
            {
                // Toggle the visibility of the first object
                isHidden1 = !isHidden1;
                objectToHide1.SetActive(!isHidden1);
                objectToHide2.SetActive(isHidden1);

                // Reset the timer
                triggerTimer = 0.0f;
            }
            else if (isHidden1 && triggerTimer >= triggerDelay)
            {
                // Toggle the visibility of the second object
                isHidden2 = !isHidden2;
                objectToHide2.SetActive(!isHidden2);
            }
        }

        if (isHidden1 && isHidden2)
        {
            objectToHide3.SetActive(false);
            Destroy(this.GetComponent<CloseUIElements>());
        }
            
        
        // Update the trigger timer
        triggerTimer += Time.deltaTime;
    }
}