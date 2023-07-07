// Created on base of youtube tutorial by Valem, extended by Krista Plagemann //
// Syncs the position of head and hands of our XRPlayer with the visuals for the network //

using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField] private Transform _Head;           // Our head seen by others in the network

    [SerializeField] private Transform _RightHand;      // Our right hand seen by others in the network
    [SerializeField] private Animator _RightAnimator;   // Animator of our left hand in the network

    [SerializeField] private Transform _LeftHand;       // Our left hand seen by others in the network
    [SerializeField] private Animator _LeftAnimator;    // Animator of our right hand in the network

    private XROrigin _myOrigin;         // Our own XROrigin 
    private PhotonView _photonView;     // Photon view component which handles syncing this obejct in the network

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        // If this Player is my own, then I hide the visuals for myself
        if (_photonView.IsMine)
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>())
                renderer.enabled = false;

            _myOrigin = FindObjectOfType<XROrigin>();
        }
    }

    private void Update()
    {
        if(_photonView.IsMine)
        {
            MapPosition(_Head, XRNode.Head);
            MapPosition(_RightHand, XRNode.RightHand);
            MapPosition(_LeftHand, XRNode.LeftHand);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), _RightAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), _LeftAnimator);

            // We need to also sync the position of this parent to our XROrigin because when we teleport or rotate etc we change the XROrigin and not the position of head/hands
            transform.position = _myOrigin.transform.position;
            transform.rotation = _myOrigin.transform.rotation;
        }
    }

    // Updates the position of the component every frame
    private void MapPosition(Transform target, XRNode node)
    {
        // We get the position and rotation of our own head/hands from InputDevices
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

        // Then we sync the position of our network objects
        target.transform.localPosition = position;
        target.transform.localRotation = rotation;
    }

    // Updates the animator of the network player hands according to our own hand input
    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            handAnimator.SetFloat("Pinch", triggerValue);
        else
            handAnimator.SetFloat("Pinch", 0);


        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            handAnimator.SetFloat("Flex", gripValue);
        else
            handAnimator.SetFloat("Flex", 0);
    }
}
