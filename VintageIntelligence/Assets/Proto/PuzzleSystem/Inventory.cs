// Created 27.05.2023 by Krista Plagemann //
// Handles picked up objects and lets you check if objects are owned. //

using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Singleton

    public static Inventory Instance { get; private set; }

    private void Awake()
    {
        transform.SetParent(null);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    #endregion

    [Tooltip("All Pickupables that exist in this project.")]
    public List<Pickupable> AllPickupables;

    private List<Pickupable> _ownedObjects = new();
    public List<Pickupable> OwnedObjects { get { return _ownedObjects; } }

    /// <summary>
    /// Triggers when any objects is picked up or discarded.
    /// </summary>
    public event Action<Pickupable, bool> OnAnyPicked = delegate { };

    #region Storing and retrieving

    private void Start()
    {
        // By subscribing to all pickupables in start, we automatically react to any of them firing the OnPicked event.
        // This way we will automatically add or remove them from the inventory when the event fires.
        foreach (var obj in AllPickupables)
        {
            obj.OnPicked += CollectObject;  // subscribes to the OnPicked event of every Pickupable in the list.
        }
    }

    /// <summary>
    /// Adds the collected pickupable to the inventory or removes it.
    /// </summary>
    /// <param name="collectedObj"></param>
    public void CollectObject(Pickupable collectedObj, bool state)
    {
        if (state)
            _ownedObjects.Add(collectedObj);           
        else
        {
            if (_ownedObjects.Contains(collectedObj))
                _ownedObjects.Remove(collectedObj);
        }

        OnAnyPicked(collectedObj, state);   // fires the event that any object was picked or discarded
    }

    /// <summary>
    /// Check if a pickupable is collected by name.
    /// </summary>
    /// <param name="objectName">Name of the pickupable stored in the SO.</param>
    /// <returns>True if collected, false if not collected yet.</returns>
    public bool CheckObjectOwned(string objectName)
    {
        if (_ownedObjects.Count == 0)   // returning if theres none stored
            return false;

        foreach (var obj in _ownedObjects)  // goes through all objects and checks if the name matches
        {
            if(obj.PickupableName == objectName)
                return true;
        }
        return false;   // only if none of the names match, this is executed
    }

    /// <summary>
    /// Check if a pickupable is collected by name and removes it from the inventory if yes.
    /// </summary>
    /// <param name="objectName">Name of the pickupable stored in the SO.</param>
    /// <returns>True if collected, false if not collected yet.</returns>
    public bool CheckObjectAndTakeIfYes(string objectName)
    {
        if(_ownedObjects.Count == 0)
            return false;

        foreach (var obj in _ownedObjects)
        {
            if (obj.PickupableName == objectName)
            {
                _ownedObjects.Remove(obj);  // remove the object from the list if it fits
                obj.Discard();
                return true;
            }
        }
        return false;
    }

    #endregion
}
