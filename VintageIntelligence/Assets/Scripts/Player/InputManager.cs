using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{
    public InputDeviceCharacteristics LeftControllerCharacteristics;
    public InputDeviceCharacteristics RightControllerCharacteristics;
    public InputDeviceCharacteristics HeadCharacteristics;
    private enum InputType { trigger, grip }

    private InputDevice _leftHand;
    private InputDevice _rightHand;
    private InputDevice _head;

    private void OnEnable()
    {
        TryInitializeController();
    }

    private void TryInitializeController()
    {
        List<InputDevice> leftDevices = new List<InputDevice>();
        // Left Hand
        InputDevices.GetDevicesWithCharacteristics(LeftControllerCharacteristics, leftDevices);
        if (leftDevices.Count > 0)
            _leftHand = leftDevices[0];
        // Right Hand
        List<InputDevice> rightDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(RightControllerCharacteristics, rightDevices);
        if (rightDevices.Count > 0)
            _rightHand = rightDevices[0];
        // Head
        List<InputDevice> headDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(HeadCharacteristics, headDevices);
        if (headDevices.Count > 0)
            _head = headDevices[0];
    }

    public InputDevice GetDevice(XRNode node)
    {
        switch (node)
        {
            case XRNode.LeftHand:
                if (_leftHand == null)
                    TryInitializeController();
                return _leftHand;
            case XRNode.RightHand:
                if (_rightHand == null)
                    TryInitializeController();
                return _rightHand;
            case XRNode.Head:
                if (_head == null)
                    TryInitializeController();
                return _head;
            default:
                return _leftHand;
        }
    }

    public float GetTriggerValueOf(XRNode node)
    {
        float triggerValue = 0;
        switch (node)
        {
            case XRNode.LeftHand:
                _leftHand.TryGetFeatureValue(CommonUsages.grip, out triggerValue); 
                break;
            case XRNode.RightHand:
                _rightHand.TryGetFeatureValue(CommonUsages.grip, out triggerValue);
                break;
        }
        return triggerValue;
    }

}
