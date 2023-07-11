using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class CameraSwitch : MonoBehaviour
{
    public string targetLayer = "Posessable";
    public XRController controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            // Check if the trigger button is pressed on the controller
            if (controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Debug.Log("Object hit and trigger button pressed!");
            }
        }
    }
}