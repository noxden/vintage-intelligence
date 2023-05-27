// Created on base of youtube tutorial by Valem //
// Extension of the XR Grab Interactable that allows for ownership takeover to let multiple players in a network grab an object. //

using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabInteractableNetwork : XRGrabInteractable
{
    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        _photonView.RequestOwnership();
        base.OnSelectEntered(args);
    }
}
