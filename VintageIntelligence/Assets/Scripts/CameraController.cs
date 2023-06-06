using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 10f;
    [SerializeField] private float bounds = 3f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), Mathf.Sin(Time.timeSinceLevelLoad * 2) * 0.3f, Input.GetAxis("Vertical"));
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        transform.position = UniformVectorClamp(transform.position, -bounds, bounds); //< To keep the camera from going out of bounds
    }

    private Vector3 UniformVectorClamp(Vector3 input, float min, float max)
    {
        Vector3 output;
        output.x = Mathf.Clamp(input.x, min, max);
        output.y = Mathf.Clamp(input.y, min, max);
        output.z = Mathf.Clamp(input.z, min, max);
        return output;
    }
}
