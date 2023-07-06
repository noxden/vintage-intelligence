// Functionality moved to PuzzleHandler

using System.Linq;
using UnityEngine;

public class WireChecker : MonoBehaviour
{
    public static WireChecker Instance { get; private set; }
    private void Awake() => Instance = this;

    [Header("Wire Objects")] public GameObject[] wireObjects;
    [Header("Rotation Objects")]
    public bool rightRotation = false;
    public WireRotation[] wireRotation;

    private void Start()
    {
        if (PuzzleHandler.Instance._ThisPlayer == PlayerType.CavePlayer)
        {
            SetWireVisibility();
        }
    }

    private void Update()
    {
        rightRotation = AllRightRotation();
    }

    private bool AllRightRotation()
    {
        return true;
       // return wireRotation.All(rotation => rotation.rightRotation != false);
    }

    private void SetWireVisibility()
    {
        foreach (GameObject wires in wireObjects)
        {
            wires.SetActive(true);
        }
    }
}
