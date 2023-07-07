// Created 07.07.2023 by Krista Plagemann //
// Toggles object visibliity (Active state) according to which player this is //

using UnityEngine;

public class TogglePlayerVisibility : MonoBehaviour
{
    [SerializeField] private PlayerType _TurnOffFor;

    void Start()
    {
        if(PuzzleHandler.Instance._ThisPlayer == _TurnOffFor)
            gameObject.SetActive(false);
    }

    public void VisibilityChangePlayerCheck( bool visibileState)
    {
        if(PuzzleHandler.Instance._ThisPlayer == _TurnOffFor)
        {
            gameObject.SetActive(visibileState);
        }
    }

}
