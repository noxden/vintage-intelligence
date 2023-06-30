using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireRotation : MonoBehaviour
{
    public bool rightRotation = false;

    public int desiredTurns;

    private int rotationNumber = 0;

    public void RotateWire()
    {
        transform.Rotate(90, 0, 0);
        if (rotationNumber < 3)
        {
            rotationNumber += 1;
        }
        else
        {
            rotationNumber = 0;
        }

        rightRotation = desiredTurns == rotationNumber;
        
    }
}
