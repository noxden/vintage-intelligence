
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class MoveHandle : MonoBehaviour
{
    [SerializeField] private Transform _grabber;
    [SerializeField] private Transform _pivot;
    public InputDeviceCharacteristics LeftControllerCharacteristics;
    public InputDeviceCharacteristics RightControllerCharacteristics;

    private bool _isGrabbable = false;
    private bool _grabbing = false;

    private Transform _grabbingHand;
    private InputDevice _leftHand;
    private InputDevice _rightHand;


    // Enable grabbing if a hand enters the trigger collider
    private async void OnTriggerEnter(Collider other)
    {
        if (_grabbing) // if we're already grabbing, we ignore this
            return;

        if (other.gameObject.CompareTag("RightHand"))
        {
            if (_rightHand == null)
            {
                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(RightControllerCharacteristics, devices);
                _rightHand = devices[0];
            }
            
            //Abort grabbing this one if we're grabbing something else.... in theory haha
            _rightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool gripState);
            if (gripState)
                return;

            _grabbingHand = other.transform;
            _isGrabbable = true;


            await UpdateHandlePosition(RightControllerCharacteristics);
        }
        
        if (other.gameObject.CompareTag("LeftHand"))
        {
            if (_leftHand == null)
            {
                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(RightControllerCharacteristics, devices);
                _leftHand = devices[0];
            }

            //Abort grabbing this one if we're grabbing something else
            _leftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool gripState);
            if (gripState)
                return;

            _grabbingHand = other.transform;
            _isGrabbable = true;

            await UpdateHandlePosition(LeftControllerCharacteristics);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RightHand") || other.gameObject.CompareTag("LeftHand"))
        {
            _isGrabbable = false;
        }
    }

    // Wait for grabbing and update the rotation of the handle
    private async Task UpdateHandlePosition(InputDeviceCharacteristics hand)
    {
        InputDevice _thisHand;

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(hand, devices);
        _thisHand = devices[0];
        _thisHand.TryGetFeatureValue(CommonUsages.gripButton, out bool gripState);
        
        while (!gripState)
        {
            _thisHand.TryGetFeatureValue(CommonUsages.gripButton, out gripState);
            Debug.Log(gripState);
            if (!_isGrabbable)
                break;
            await Task.Yield();
        }

        if (gripState)
        {
            _grabbing = true;

            Debug.LogWarning("Start grabbed");
            while (_grabbing)
            {
                Vector3 handPosition = new Vector3(_pivot.position.x, _grabbingHand.position.y, _grabbingHand.position.z);
                Vector3 grabberPosition = new Vector3(_pivot.position.x, _grabber.position.y, _grabber.position.z);
                //float angleTowards = Vector3.Angle(handPosition - _pivot.position, _grabber.transform.position - _pivot.position);
                float angleTowards = SignedAngleBetween(handPosition - _pivot.position, grabberPosition - _pivot.position, _pivot.right);
                //float angleTowards = Vector3.Angle(Vector3.Normalize(handPosition - _pivot.position), _grabber.transform.position - _pivot.position);

                Debug.DrawRay(_pivot.position, grabberPosition - _pivot.position, Color.red);
                Debug.DrawRay(_pivot.position, handPosition - _pivot.position, Color.blue);
                transform.RotateAround(_pivot.position, Vector3.right, angleTowards);
                //yield return new WaitForSeconds(1);
                _thisHand.TryGetFeatureValue(CommonUsages.gripButton, out gripState);

                if (!gripState)
                {
                    _grabbing = false;
                    break;
                }
                await Task.Yield();
            }
        }
    }

    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (not used but included here for completeness)
        //float angle360 =  (signed_angle + 180) % 360;

        return signed_angle;
    }

}
