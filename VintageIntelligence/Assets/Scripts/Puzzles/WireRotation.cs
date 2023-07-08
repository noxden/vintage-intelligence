// Created by Max von Trümbach, edited by Krista Plagemann //
// Manages rotation of the wire components and marking itself done when correctly rotated.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WireRotation : MonoBehaviour
{
    //public bool rightRotation = false;
    //public int desiredTurns;

    [SerializeField, TextArea(0, 4)]
    public string RotationValues = "0 = Upwards, 1 = Right, 2 = Downwards, 3 = Left";

    [SerializeField] private int _CurrentRotation = 0;
    [SerializeField] private int _DesiredRotation = 0;

    // In case it is a mirrored piece and it can have 2 rotations
    [SerializeField] private bool _TwoRotationsPossible = false;
    [SerializeField] private int _AlternativeDesired = 0;

    [SerializeField] private MeshRenderer _MeshRenderer;
    [SerializeField] private int _PositionOfBaseMat = 2;

    private bool _connectedRight = false;

    /// <summary>
    /// Rotates this objectin the given direction.
    /// </summary>
    /// <param name="direction"> -1 for left, 1 for right</param>
    public void RotateWire(int direction)
    {
        if (direction == -1)
        {
            transform.Rotate(0, -90, 0);
            // Save the rotation as left if we are upwards
            if(_CurrentRotation == 0)
                _CurrentRotation = 3;
            else // else minuses it
                _CurrentRotation += direction;
        }
        else if (direction == 1)
        {
            transform.Rotate(0, 90, 0);

            // Save the rotation as upwards if we are left
            if (_CurrentRotation == 3)
                _CurrentRotation = 0;
            else // else adds it
                _CurrentRotation += direction;
        }

        // If the wire is rotated right
        if (_CurrentRotation == _DesiredRotation)
        {
            _connectedRight = true;
            PuzzleHandler.Instance.SetWirePlaced(true);
        }
        else if(_TwoRotationsPossible && _CurrentRotation == _AlternativeDesired)
        {
            _connectedRight = true;
            PuzzleHandler.Instance.SetWirePlaced(true);
        }
        else if(_connectedRight) // if it WAS rotated right, but isn't now
        {
            PuzzleHandler.Instance.SetWirePlaced(false);
            _connectedRight = false;
        }
    }

    public void FinishWire(Material finishMat)
    {
        List<Material> materials = new();
        foreach (var mat in _MeshRenderer.materials)
            materials.Add(mat);

        materials[_PositionOfBaseMat] = finishMat;
        _MeshRenderer.SetMaterials(materials);
        GetComponent<Collider>().enabled = false;
    }
}
