// Created 13.07.2023 by Krista Plagemann //
// Collects a car key by dropping it into a collection zone.
// Due to time reasons, this script is terribly but it works :D


using UnityEngine;

public class Inv_DropCollected : MonoBehaviour
{
    [SerializeField] private Pickupable _thisPickupable;
    private bool _isInZone = false;

    public void SetInZone(bool state)
    {
        _isInZone = state;
    }

    public void DroppedCheckInZone(GameObject pickupableObject)
    {
        if (_isInZone)
        {
            pickupableObject.SetActive(false);
            _thisPickupable.PickThisUp();
        }
    }

}
