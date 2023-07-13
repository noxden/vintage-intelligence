// Created 27.05.2023 by Krista Plagemann //

// Pickupable that can be stored in the Inventory. //

using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Pickupable", menuName = "VI/Pickupable", order = 0)]
public class Pickupable : ScriptableObject
{
    [field: SerializeField, Tooltip("Name of this pickup to check if owned.")]
    public string PickupableName { get; private set; }

    [field: SerializeField, Tooltip("Icon for the inventory display.")]
    public Sprite Icon { get; private set; }

    /// <summary>
    /// Triggers with true when object picked up and false if discarded.
    /// </summary>
    public event Action<Pickupable, bool> OnPicked = delegate { };

    public void PickThisUp()
    {
        OnPicked?.Invoke(this, true);
    }

    public void Discard()
    {
        OnPicked?.Invoke(this, false);
    }
}
