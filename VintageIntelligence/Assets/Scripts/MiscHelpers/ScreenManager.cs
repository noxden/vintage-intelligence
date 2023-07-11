// Created by Krista Plagemann and Max von Trümbach //
// Turns on the displays in the built version once started. //

using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    
    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }  
    }

}
