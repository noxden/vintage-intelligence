using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleHandler : MonoBehaviour
{
    public static PuzzleHandler Instance { get; private set; }

    private void Awake() => Instance = this;
    

    public int BigClockHandlesCorrect = 0;
    public int Clock1HandlesCorrect = 0;
    public int Clock2HandlesCorrect = 0;
    public int Clock3HandlesCorrect = 0;

    public UnityEvent<int, bool> ClockDone;
    public UnityEvent AllClocksDone;

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
        }

    }
}
