// Created 07.06.2023 by Krista Plagemann//
// Changes the visuals of a posessable object during hover. //

using UnityEngine;

public class PosessableHighlight : MonoBehaviour
{
    [SerializeField] private Material _selectMat;

    private Material _baseMat;
    private MeshRenderer _meshRenderer;

    public int IndexOfObject;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _baseMat = _meshRenderer.material;
    }

    public void SelectObject(bool state)
    {
        if (state)
            _meshRenderer.material = _selectMat;
        else
            _meshRenderer.material = _baseMat;
    }
}
