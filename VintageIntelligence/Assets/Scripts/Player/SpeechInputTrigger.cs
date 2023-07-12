// Created 31.05.2023 by Krista Plagemann //
// Manages controller input to start a speech recording (trigger & grip buttons). //


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class SpeechInputTrigger : MonoBehaviour
{
    public InputDeviceCharacteristics controllerToUse;

    public UnityEvent OnStartedRecording;
    public UnityEvent OnFinishedRecording;
    public event Action<float> OnSecondsIntoRecording;

    private enum InputType { trigger, grip }

    private InputDevice _leftHand;
    private bool _speechInputActive = true;
    private float _secondsRecorded = 30f;

    private void SetSpeechInputActive(bool state) { _speechInputActive = state; }


    private Coroutine _waitForButtonRoutine = null;

    private void Start()
    {
        InputStatusManager.Instance.OnSpeechInputToggle += SetSpeechInputActive;
        TryInitializeController();
    }

    private void TryInitializeController()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerToUse, devices);
        if (devices.Count > 0)
            _leftHand = devices[0];
    }

    private void Update()
    {
        if (_speechInputActive && _waitForButtonRoutine == null)
        {
            if (!_leftHand.isValid)
                TryInitializeController();

            // Left hand Trigger
            _leftHand.TryGetFeatureValue(CommonUsages.trigger, out float lTriggerPressed);
            if (lTriggerPressed > 0.3)
                _waitForButtonRoutine = StartCoroutine(WaitForSecondButton(InputType.trigger));
            

            // Left hand Grip
            _leftHand.TryGetFeatureValue(CommonUsages.grip, out float lGripPressed);
            if (lGripPressed > 0.3)
                _waitForButtonRoutine = StartCoroutine(WaitForSecondButton(InputType.grip));


        }
    }

    private IEnumerator WaitForSecondButton(InputType currentlyPressed)
    {
        bool oneButtonActive = true;

        while (oneButtonActive)
        {
            if (currentlyPressed == InputType.trigger)
            {
                _leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out oneButtonActive);
                _leftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool otherButtonActive);


                if (oneButtonActive && otherButtonActive)
                {
                    SpeechManager.StartSpeechRecording();
                    StartCoroutine(WaitForEndButton());
                    yield break;
                }
            }
            else
            {
                _leftHand.TryGetFeatureValue(CommonUsages.gripButton, out oneButtonActive);
                _leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool otherButtonActive);

                if (oneButtonActive && otherButtonActive)
                {
                    SpeechManager.StartSpeechRecording();
                    StartCoroutine(WaitForEndButton());
                    yield break;
                }
            }
            yield return null;
        }

        _waitForButtonRoutine = null;
    }

    private IEnumerator WaitForEndButton()
    {
        OnStartedRecording?.Invoke();
        bool oneButtonActive = true;
        bool otherButtonActive = true;
        _secondsRecorded = 30f;
        while (oneButtonActive && otherButtonActive)
        {
            OnStartedRecording?.Invoke();
            _secondsRecorded -= Time.deltaTime;
            OnSecondsIntoRecording?.Invoke(_secondsRecorded);
            _leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out oneButtonActive);
            _leftHand.TryGetFeatureValue(CommonUsages.gripButton, out otherButtonActive);

            yield return null;
        }

        OnFinishedRecording?.Invoke();
        _waitForButtonRoutine = null;
        SpeechManager.StopSpeechRecording();
    }
}
