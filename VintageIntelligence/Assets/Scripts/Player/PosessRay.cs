// Created 07.06.2023 by Krista Plagemann//
// Let's the cave player teleport to objects on the Posessable layer by using the right trigger button //

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PosessRay : MonoBehaviour
{
    [SerializeField] private LayerMask _PosessableLayer;
    [SerializeField] private InputDeviceCharacteristics controllerToUse;
    [SerializeField] private Transform CavePlayer;

    private InputDevice _rightHand;
    private Transform _rayOrigin;
    private GameObject _lastHit;
    private GameObject _lastSelected;

    private bool _teleportOn = true;
    public void SetTeleportState(bool state) { _teleportOn = state; }

    private void Start()
    {
        CreateRayOrigin();
        TryInitializeController();
    }

    // Creates an origin point at the hand position. //
    private void CreateRayOrigin()
    {
        _rayOrigin = new GameObject($"[{gameObject.name}] Ray Origin").transform;
        _rayOrigin.SetParent(transform, false);
        _rayOrigin.localPosition = Vector3.zero;
        _rayOrigin.localRotation = Quaternion.identity;
    }

    // Gets the controller from InputDecives //
    private void TryInitializeController()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerToUse, devices);
        if (devices.Count > 0)
            _rightHand = devices[0];
    }


    private void FixedUpdate()
    {
        if (_teleportOn)
        {
            RaycastHit hit;

            // Casts a ray and checks if we hit an object on the right layer 
            if (Physics.Raycast(_rayOrigin.position, _rayOrigin.forward, out hit, Mathf.Infinity, _PosessableLayer))
            {
                // If this is the first time we hit this object, we save it and trigger some visual changes on it to indicate we are aiming on it (a hover basically)
                if (_lastHit == null)
                {
                    _lastHit = hit.collider.gameObject;
                    _lastHit.GetComponent<PosessableHighlight>().SelectObject(true);
                }
                // Make sure we have a controller connected for the rest of the code
                if (!_rightHand.isValid)
                {
                    TryInitializeController();
                    if (!_rightHand.isValid)
                        return;
                }

                // Get the trigger value to check if trigger is pressed
                _rightHand.TryGetFeatureValue(CommonUsages.trigger, out float rTriggerPressed);

                // If trigger is pressed
                if (rTriggerPressed > 0.3)
                {
                    if(_lastSelected!= null)
                        _lastSelected.SetActive(true);  // Reactivate the object we were posessing previously if there is one

                    TeleportTo(_lastHit.transform); // teleport to its position
                    _lastSelected = _lastHit;   // save the selected object we just teleported to, so we can turn it on again after we exit
                    _lastSelected.GetComponent<PosessableHighlight>().SelectObject(false);  // revert back from the hover visuals
                    _lastSelected.SetActive(false); // and deactivate the object we are currently posessing
                }                   

                Debug.DrawRay(_rayOrigin.position, _rayOrigin.forward * hit.distance, Color.yellow);
            }
            else // if there is no object hit we make sure last hit is null if it isn't yet
            {
                if(_lastHit != null)
                {
                    _lastHit.GetComponent<PosessableHighlight>().SelectObject(false);
                    _lastHit = null;
                }
                Debug.DrawRay(_rayOrigin.position, _rayOrigin.forward * 1000, Color.white);
            }
        }
    }

    private void TeleportTo(Transform hitObject)
    {
        CavePlayer.position = new Vector3(hitObject.position.x, 0.0f, hitObject.position.z);    // change our position to the objects position, but the height should be 0 always
        CavePlayer.forward = hitObject.forward; // turn so we are looking forward
    }

}
