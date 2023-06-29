// Created by Krista Plagemann//
// Checks for the handles entering to collider to signal that we set the time correctly. //

using UnityEngine;

public class SetClock : MonoBehaviour
{
    [SerializeField] private int ClockIndex = 0;
    [SerializeField] private GameObject HandleObject;

    [SerializeField] private bool HandleCorrect = false;

    [SerializeField] private bool ClockVisualHandler = false;
    [SerializeField] private Renderer ClockVisuals;



    private void Start()
    {
        if (ClockVisualHandler)
            PuzzleHandler.Instance.ClockDone.AddListener(SetClockDone);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == HandleObject)
        {
            PuzzleHandler.Instance.SetClockState(ClockIndex, true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == HandleObject)
        {
            PuzzleHandler.Instance.SetClockState(ClockIndex, false);
        }
    }

    private void SetClockDone(int index, bool state)
    {
        if(index == ClockIndex)
        {
            if(state)
                ClockVisuals.material.color = Color.green;
            else
                ClockVisuals.material.color = Color.white;
        }
    }
}
