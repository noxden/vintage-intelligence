using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterEvent : MonoBehaviour
{
    [SerializeField] private string TagToCheck;
    [SerializeField] private bool CheckForObject = false;
    [SerializeField] private GameObject ObjectToCheck;

    public UnityEvent OnEntered;
    public UnityEvent OnExited;

    private void OnTriggerEnter(Collider other)
    {
        if(CheckForObject)
        {
            if (other.gameObject == ObjectToCheck)
                OnEntered?.Invoke();
        }
        else if(other.CompareTag(TagToCheck))
            OnEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (CheckForObject)
        {
            if (other.gameObject == ObjectToCheck)
                OnExited?.Invoke();
        }
        else if(other.CompareTag(TagToCheck))
            OnExited?.Invoke();
    }
}
