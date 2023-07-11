using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechDebugUI : MonoBehaviour
{
    [SerializeField] private SpeechInputTrigger _SpeechInput;

    [SerializeField] private GameObject _RecordingWindow;
    [SerializeField] private GameObject _SpeechWindow;

    [SerializeField] private Slider _Slider;
    [SerializeField] private TextMeshProUGUI _SecondsLeft;

    [SerializeField] private TextMeshProUGUI _RecognizedText;
    [SerializeField] private TextMeshProUGUI _SpokenText;

    void Start()
    {
        SpeechManager.Instance.OnNewRecognizedText += NewRecognizedText;
        SpeechManager.Instance.OnNewSpokenText += NewSpokenText;
        _SpeechInput.OnSecondsIntoRecording += ShowSecondsRecording;
        _SpeechInput.OnStartedRecording.AddListener(ShowRecordingUI);
        _SpeechInput.OnFinishedRecording.AddListener(HideRecordingUI);
    }

    private void NewRecognizedText(string message)
    {
        _RecognizedText.SetText(message);
    }
    
    private void NewSpokenText(string message)
    {
        _SpokenText.SetText(message);
    }

    private void ShowRecordingUI()
    {
        _RecordingWindow.SetActive(true);
        _SpeechWindow.SetActive(false);
    }
    
    private void HideRecordingUI()
    {
        _RecordingWindow.SetActive(false);
        _SpeechWindow.SetActive(true);
    }

    private void ShowSecondsRecording(float seconds)
    {
        if (seconds >= 0)
        {
            _Slider.value = seconds;
            _SecondsLeft.SetText(seconds.ToString());
        }
    }
}
