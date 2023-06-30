// Created by Krista Plagemann//
// Simulates that the handles are grabbable (via this script instead of XRGrabInteractable bcs it just wasn't working well). //
// Calculates rotation of the handle following the hand movement in clock-like rotations (it's not actually grabbable lol). //

using Photon.Pun;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class MoveHandle : MonoBehaviour
{
    [SerializeField] private Transform _grabber;    // Position of the grabbable sphere
    [SerializeField] private Transform _pivot;      // pivot in the center of the clock to get the axis of rotation
    [SerializeField] private PhotonView _photonViewHandle;  // just to get ownership over the transform

    public InputDeviceCharacteristics LeftControllerCharacteristics;
    public InputDeviceCharacteristics RightControllerCharacteristics;
    private InputDevice _leftHand;
    private InputDevice _rightHand;

    private bool _isGrabbable = false;  // if the hand is inside the grab zone
    private bool _grabbing = false;     // and if we are pressing the grab button
    private Transform _grabbingHand;    // Hand that is grabbing the handle sphere


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

            _photonViewHandle.RequestOwnership();

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
            if (!_isGrabbable)
                break;
            await Task.Yield();
        }

        if (gripState)
        {
            _grabbing = true;

            while (_grabbing)
            {
                // Calculates the angle between where the handle points right now (grabber position) and the hand position.
                // Then we rotate to make the handle rotation(this rotation) the same as the hand position so it follows the hands(we are faking the grabbing lol)
                Vector3 handPosition = new Vector3(_pivot.position.x, _grabbingHand.position.y, _grabbingHand.position.z);
                Vector3 grabberPosition = new Vector3(_pivot.position.x, _grabber.position.y, _grabber.position.z);

                float angleTowards = SignedAngleBetween(handPosition - _pivot.position, grabberPosition - _pivot.position, _pivot.right, false);


                Debug.DrawRay(_pivot.position, grabberPosition - _pivot.position, Color.red);
                Debug.DrawRay(_pivot.position, handPosition - _pivot.position, Color.blue);
                transform.RotateAround(_pivot.position, Vector3.right, angleTowards);

                _thisHand.TryGetFeatureValue(CommonUsages.gripButton, out gripState);   // always check if we are still grabbing

                if (!gripState)
                {
                    _grabbing = false;
                    break;
                }
                await Task.Yield();
            }
        }
    }

    public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n, bool fullAngle)
    {
        // angle in [0,180]
        float angle = Vector3.Angle(a, b);
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        // angle in [0,360] (use bool to use :))
        if(fullAngle)
            signed_angle =  (signed_angle + 180) % 360;

        return signed_angle;
    }

}
