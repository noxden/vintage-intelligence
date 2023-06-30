// Created 15.06.2023 by Krista Plagemann//
// Handles the puzzles and collection and events related to each puzzle

using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public enum PlayerType { CavePlayer, HMDPlayer }

public class PuzzleHandler : MonoBehaviourPunCallbacks
{
    public static PuzzleHandler Instance { get; private set; }

    private void Awake() => Instance = this;

    public PlayerType _ThisPlayer;

    public int HandlesPlaced = 0;
    public GameObject[] GhostHandlesForCP;

    public int BigClockHandlesCorrect = 0;
    public int Clock1HandlesCorrect = 0;
    public int Clock2HandlesCorrect = 0;
    public int Clock3HandlesCorrect = 0;

    public UnityEvent<int, bool> ClockDone;
    public UnityEvent AllClocksDone;
    public UnityEvent AllHandlesCollected;
    public UnityEvent SeedPlanted;
    public UnityEvent OnSeedWatered;

    #region Clocks Puzzle

    [SerializeField] private GameObject _BrickToPush;
    [SerializeField] private Transform _ForcePushPoint;

    public void BrickPushedReleasedThis()
    {
        _BrickToPush.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3 (0,0,20), _ForcePushPoint.position, ForceMode.Impulse);
    }

    public void AddHandleCollected()
    {
        HandlesPlaced++;
        if (HandlesPlaced == 2)
        {
            AllHandlesCollected?.Invoke();        
            photonView.RPC("AllHandlesDone", RpcTarget.Others);
            SetHandleVisibilityForCP(true);
        }
    }

    private void SetHandleVisibilityForCP(bool state)
    {
        if(_ThisPlayer == PlayerType.CavePlayer)
        {
            foreach(GameObject handle in GhostHandlesForCP)
            {
                handle.SetActive(state);
            }
        }
    }

    public void SetClockState(int clockIndex, bool state)
    {
        switch(clockIndex)
        {
            case 0:
                if (state)
                    BigClockHandlesCorrect++;
                else
                    BigClockHandlesCorrect--;

                if(BigClockHandlesCorrect == 2)
                    ClockDone?.Invoke(0, true);
                else
                    ClockDone?.Invoke(0, false);
                break;

            case 1:
                if (state)
                    Clock1HandlesCorrect++;
                else
                    Clock1HandlesCorrect--;

                if (Clock1HandlesCorrect == 2)
                    ClockDone?.Invoke(1, true);
                else
                    ClockDone?.Invoke(1, false);
                break;

            case 2:
                if (state)
                    Clock2HandlesCorrect++;
                else
                    Clock2HandlesCorrect--;

                if (Clock2HandlesCorrect == 2)
                    ClockDone?.Invoke(2, true);
                else
                    ClockDone?.Invoke(2, false);
                break;

            case 3:
                if (state)
                    Clock3HandlesCorrect++;
                else
                    Clock3HandlesCorrect--;

                if (Clock3HandlesCorrect == 2)
                    ClockDone?.Invoke(3, true);
                else
                    ClockDone?.Invoke(3, false);
                break;
        }

        if(BigClockHandlesCorrect + Clock1HandlesCorrect + Clock2HandlesCorrect + Clock3HandlesCorrect == 8)
        {
            AllClocksDone.Invoke();
            photonView.RPC("AllClocksDoneNetwork", RpcTarget.Others);
            SetHandleVisibilityForCP(false);
        }

    }

    #endregion


    #region Plant

    private bool _seedIsPlanted = false;
    private bool _seedIsWatered = false;

    // When the plant is dropped in the planting zone
    public void PlantSeed()
    {
        SeedPlanted?.Invoke();
        _seedIsPlanted = true;
        photonView.RPC("SeedPlantedNetwork", RpcTarget.Others);   
    }

    // When the water stream successfully hits the watering zone.
    public void SeedWatered()
    {
        if (_seedIsWatered || !_seedIsPlanted)
            return;
        _seedIsWatered = true;
        OnSeedWatered?.Invoke();
        photonView.RPC("SeedWateredNetwork", RpcTarget.Others);
    }


    #endregion

    // Notifies the network player(cave player) when a puzzle is done so the visual effects also take place for them

    [PunRPC]
    private void AllHandlesDone()
    {
        AllHandlesCollected?.Invoke();
        SetHandleVisibilityForCP(true);
    }

    [PunRPC]
    private void AllClocksDoneNetwork()
    {
        AllClocksDone.Invoke();
        SetHandleVisibilityForCP(false);
    }

    [PunRPC]
    private void SeedPlantedNetwork()
    {
        SeedPlanted?.Invoke();
    }
    
    [PunRPC]
    private void SeedWateredNetwork()
    {
        OnSeedWatered?.Invoke();
    }

}
