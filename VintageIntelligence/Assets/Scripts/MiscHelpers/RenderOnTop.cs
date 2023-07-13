// Created 13.07.2023 by Krista Plagemann //
// Little helper to render stuff on top always

using UnityEngine;
using UnityEngine.Rendering;

public class RenderOnTop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.renderQueue = ((int)RenderQueue.Overlay);
            //renderer.material. = null;
        }
    }

}
