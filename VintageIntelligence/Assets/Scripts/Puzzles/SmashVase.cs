// Created by Krista Plagemann//
// Handles a smashable vase by calculating velocity (distance traveled over time)

using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class SmashVase : MonoBehaviour
{
    private MeshRenderer _thisVase;
    private Collider _collider;
    [SerializeField] private GameObject _SmashedObjects;
    [SerializeField] private GameObject _Handle;
    [SerializeField] private float _MinDistance = 0.5f;
    [SerializeField] private float _MaxTimeTraveled = 0.1f;

    private Vector3 hitpos;

    private bool checkDistance = false;
    private float distanceTraveledUntilCrash = 0;

    private void Start()
    {
        hitpos = transform.position;
        _thisVase = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        GetComponent<XRGrabInteractableNetwork>().selectExited.AddListener(Released);
        GetComponent<XRGrabInteractableNetwork>().selectEntered.AddListener(InHand);
    }

    private void InHand(SelectEnterEventArgs selectEnterEventArgs)
    {
        checkDistance = false;
        distanceTraveledUntilCrash = 0;
    }

    private void Released(SelectExitEventArgs selectExitEventArgs)
    {
        checkDistance = true;
        hitpos = transform.position;
    }
    void Update()
    {
        if (checkDistance)
            distanceTraveledUntilCrash += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log($"Distance traveled: {Vector3.Distance(transform.position, hitpos)} bigger than {_MinDistance}, Time passed: {distanceTraveledUntilCrash} smaller than {_MaxTimeTraveled}");
        if (Vector3.Distance(transform.position, hitpos) >= _MinDistance)
        {
            if (distanceTraveledUntilCrash <= _MaxTimeTraveled && distanceTraveledUntilCrash > 0)
            {
                _thisVase.enabled = false;
                _collider.enabled = false;
                _SmashedObjects.SetActive(true);
                _SmashedObjects.GetComponent<PhotonView>().RequestOwnership();
                _Handle.transform.parent = null;
                _Handle.GetComponent<PhotonView>().RequestOwnership();
                _Handle.SetActive(true);
                _SmashedObjects.transform.parent = null;
                Invoke("EnableHandle", 1f);
            }
        }
    }

    private void EnableHandle()
    {
        _Handle.GetComponent<Rigidbody>().isKinematic = false;
        _Handle.GetComponent<Collider>().enabled = true;
        _Handle.GetComponent<XRGrabInteractableNetwork>().enabled = true;
    }
}
