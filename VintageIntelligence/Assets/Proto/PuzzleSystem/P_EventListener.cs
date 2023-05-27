// Created 27.05.2023 by Krista Plagemann //

// Simple event listener for the state of a pickupable. //

using UnityEngine;
using UnityEngine.Events;

public class P_EventListener : MonoBehaviour
{
    [SerializeField] private Pickupable _pickupable;           // the Pickupable this refers to

    [Tooltip("Event gets fired when we picked up the object.")]
    public UnityEvent OnPickedUp = new();

     [Tooltip("Event gets fired when we discard theo bject.")]
    public UnityEvent OnDiscarded = new();

    /// Add or remove to the objective delegates. ///
    private void OnEnable()
    {
        _pickupable.OnPicked += OnObjectivePick;
    }


    private void OnDisable()
    {
        _pickupable.OnPicked -= OnObjectivePick;
    }

    /// When the objectives state changes, we invoke events based on that ///
    private void OnObjectivePick(Pickupable pickupable, bool state)
    {
        if (state)
            OnPickedUp?.Invoke();
        else
            OnDiscarded?.Invoke();
    }
}
