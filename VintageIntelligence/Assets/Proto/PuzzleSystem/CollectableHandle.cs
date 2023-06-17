// Created 15.06.2023 by Krista Plagemann//
// Lets you put this handle in a specific collider. Function called on release of grab interactable.

using UnityEngine;

public class CollectableHandle : MonoBehaviour
{
    [SerializeField] private Collider _TargetPositionCollider;
    [SerializeField] private GameObject _TargetObjectToActivate;
    [SerializeField] private GameObject _GhostToHide;

    public void CheckIfAtTargetPosition()
    {
        if (_TargetPositionCollider.bounds.Contains(transform.position))
        {
            _TargetObjectToActivate.SetActive(true);
            _GhostToHide.SetActive(false);
            PuzzleHandler.Instance.AddHandleCollected();
            Destroy(gameObject);
        }
    }
}
