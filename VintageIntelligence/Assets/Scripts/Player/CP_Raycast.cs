// Created 05.07.2023 by Krista Plagemann//
// Makes a raycast, turns on the ray if an interactable object is hit and triggers the right functions while. //

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public enum CPInteractableType { Possessable, Wire, WireSelect}

public class CP_Raycast : MonoBehaviour
{
    [SerializeField] private LayerMask _InteractableLayers;
    [SerializeField] private InputDeviceCharacteristics controllerToUse;
    [SerializeField] private GameObject PosessRayVisual;

    [SerializeField] private float TriggerStartThreshold = 0.6f;
    [SerializeField] private float TriggerReleaseThreshold = 0.1f;

    private InputDevice _rightHand;
    private Transform _rayOrigin;

    private bool _rayActive = false;
    private bool _checkRay = true;


    private async void Start()
    {
        CreateRayOrigin();
        TryInitializeController();
        await CastRay();
    }

    public async void SetRayCheck(bool state)
    {
        if(!_checkRay && state)
            await CastRay();

        _checkRay = state;
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


    //////////////////
    // Raycast part //


    private GameObject _hoverHit;
    private GameObject _lastSelected;
    private CPInteractable _interactable;

    private async Task CastRay()
    {
        _checkRay = true;

        while (_checkRay)
        {
            if (_rayOrigin == null)
                return;

            // Turns on or off ray visuals 
            if (_rayActive)
                PosessRayVisual.SetActive(true);
            else
                PosessRayVisual.SetActive(false);


            // Make sure we have a controller connected for the rest of the code
            if (!_rightHand.isValid)
            {
                TryInitializeController();
                if (!_rightHand.isValid)
                    await Task.Yield();
            }


            // Get the trigger value to check if trigger is pressed
            _rightHand.TryGetFeatureValue(CommonUsages.trigger, out float rTriggerPressed);

            RaycastHit hit;

            // Casts a ray and checks if we hit an object on the right layer 
            if (Physics.Raycast(_rayOrigin.position, _rayOrigin.forward, out hit, Mathf.Infinity, _InteractableLayers))
            {
                // If we hit an interactable (if this is the first time it will be filled next frame)
                if (_interactable != null)
                {
                    _rayActive = true;

                    // If we still have an interactable but are hitting a new one (happens if we keep trigger hold while moving around) we replace it
                    if(_interactable != hit.collider.gameObject)
                        _interactable = hit.collider.gameObject.GetComponent<CPInteractable>();

                    // If this is the first time we hit this object, we save it and trigger some visual changes on it to indicate we are aiming on it (a hover basically)
                    if (_hoverHit == null)
                    {
                        _hoverHit = hit.collider.gameObject;
                        _interactable.StartHover();
                    }

                    // If trigger is pressed
                    if (rTriggerPressed > TriggerStartThreshold && _lastSelected == null)
                    {
                        _lastSelected = hit.collider.gameObject;
                        _interactable.StartSelect(_rightHand);
                    }

                    // If trigger is released
                    if (rTriggerPressed <= TriggerReleaseThreshold)
                    {
                        if (_lastSelected != null)
                        {
                            _interactable.EndSelect(_rightHand);
                            _lastSelected = null;
                        }
                    }
                }
                else // if not we check if we really did not hit an interactable lol
                {
                    _interactable = hit.collider.gameObject.GetComponent<CPInteractable>();
                }              

            }
            else if (_lastSelected != null && rTriggerPressed <= TriggerReleaseThreshold) // If we are still holding trigger, only release the select once we release trigger
            {
                _interactable.EndSelect(_rightHand);    // deselects the current interactable (could be different if we start select on another object i think?)
                _lastSelected.GetComponent<CPInteractable>().EndSelect();   // deselects last selected
                _lastSelected = null;
                // Only reset the interactable if we don't hover over it anymore
                if(_hoverHit == null)
                    _interactable = null;
            }
            else if (_hoverHit != null) // if there is no object hit but still one stored we make sure last hover is null
            {
                // And wem ake sure we unhover the object
                if( _interactable != null)
                {
                    _interactable.EndHover();
                    _hoverHit = null;
                    // Only reset the interactable if we don't have it selected anymore
                    if(_lastSelected == null)
                        _interactable = null;
                }
            }
            else
                _rayActive = false;

            // If trigger is pressed without hitting something we also set it on
            if (rTriggerPressed > TriggerStartThreshold)
                _rayActive = true;

            await Task.Yield();
        }

    }

    private void OnDisable()
    {
        _checkRay = false;
    }
}
