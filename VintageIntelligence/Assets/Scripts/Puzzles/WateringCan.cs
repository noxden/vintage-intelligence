// Created 29.06.2023 by Krista Plagemann//
// Calculates pouring angles of the watering can and enables a water stream if over a threshold. //
// This should do so much more to make sense and be realistic but time is not unlimited sadly :( //

using System.Threading.Tasks;
using UnityEngine;

public class WateringCan : MonoBehaviour
{
    [SerializeField, Range(0, 90), Tooltip("Minimum angle at which we start pouring(horizontal is 0, vertical is 90).")]
    private float _MinimumPouringAngle;
    [SerializeField] private GameObject _PouringWater;  // object that shows the pouring water
    [SerializeField] private Transform _Nozzle;     // nozzle where the water pours out for forward transform calculation
    [SerializeField] private Transform _TransformReference;     // An empty transform that we can make follow then nozzle for horizontal reference
    [SerializeField] private GameObject _PlantPot;  

    private bool _waterFull = false; // if we filled the can with water
    private bool _checkTilt = false;    // if we should check (only while holding)

    public async void FillWaterCan()
    {
        _waterFull = true;
        await TiltChecking();
    }
    
    // The following 2 functions should be called in the XR Grab interactable on grab and release
    public async void StartCheckTilt()
    {
        _checkTilt = true;
        await TiltChecking();
    }

    public void StopCheckTilt() { _checkTilt = false;}

    // Caluclates the angle tilt downward to turn on pouring water or not.
    private async Task TiltChecking()
    {
        while (_waterFull && _checkTilt)
        {
            // moves the reference world forward to point in the nozzle direction while staying horizontal
            _TransformReference.position = transform.position;
            _TransformReference.LookAt(new Vector3(_Nozzle.position.x, _TransformReference.position.y, _Nozzle.position.z));

            // angle on the up down axis between the nozzle foward and the world forward
            float angleTowards = - (180 - MoveHandle.SignedAngleBetween(_TransformReference.forward, _Nozzle.forward, _Nozzle.right, true)); // this is weird but... works fair enough for now

            Debug.DrawRay(_TransformReference.position, _TransformReference.forward, Color.red);
            Debug.DrawRay(_Nozzle.position, _Nozzle.forward, Color.blue);

            if (angleTowards >= _MinimumPouringAngle && angleTowards <= 180)
                _PouringWater.SetActive(true);
            else
                _PouringWater.SetActive(false);

            await Task.Yield();
        }
    }


    private void OnDisable()
    {
        _waterFull = false;
    }
}
