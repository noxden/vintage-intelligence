// https://roystan.net/articles/toon-water/ //

using UnityEngine;

public class CameraDepthTextureMode : MonoBehaviour
{

    public DepthTextureMode depthTextureMode = DepthTextureMode.Depth;

    private void OnValidate()
    {
        SetCameraDepthTextureMode();
    }

    private void Awake()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
