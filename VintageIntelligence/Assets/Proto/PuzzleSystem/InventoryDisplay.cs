// Created 27.05.2023 by Krista Plagemann //

// manages an Inventory display that can be opened via button and containst he picked up items. //

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _DisplayWindow;

    // Debug
    [SerializeField] private TextMeshProUGUI _PickupablesCounter;
    private int _amountOwned = 0;
    //

    private Inv_Collectable[] _CollectedPlaceholders;
    private Dictionary<Pickupable, GameObject> _CollectedObjects = new();

    private void Awake() => _CollectedPlaceholders = GetComponentsInChildren<Inv_Collectable>(true);

    private void Start()
    {
        Inventory.Instance.OnAnyPicked += UpdateInventoryDisplay;   
    }

    private void UpdateInventoryDisplay(Pickupable pickupable, bool state)
    {
        // Debug Test
        if (state)
            _amountOwned++;
        else
            _amountOwned--;

        _PickupablesCounter.SetText(_amountOwned.ToString());
        //

        if (state)
        {
            for (int i = 0; i < _CollectedPlaceholders.Length; i++)
            {
                if (!_CollectedObjects.ContainsValue(_CollectedPlaceholders[i].gameObject))
                {               
                    Inv_Collectable newIconUnlocked = _CollectedPlaceholders[i];
                    newIconUnlocked.SetCollected(pickupable);
                    _CollectedObjects.Add(pickupable, newIconUnlocked.gameObject);
                    return;
                }
            }
        }
        else
        {
            if (_CollectedObjects.ContainsKey(pickupable))
            {
                _CollectedObjects.TryGetValue(pickupable, out GameObject oldDiscardedObject);

                oldDiscardedObject.GetComponent<Inv_Collectable>().SetDiscarded();
                _CollectedObjects.Remove(pickupable);
                return;
            }
        }

    }

    private bool _displayVisibile = false;
    public void ToggleDisplay()
    {
        _displayVisibile = !_displayVisibile;
        _DisplayWindow.SetActive(_displayVisibile);
    }
}

