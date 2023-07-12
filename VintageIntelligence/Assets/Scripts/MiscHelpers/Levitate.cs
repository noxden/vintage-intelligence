// Created by Krista Plagemann, 21.12.2022 //
// Moves an object up and down via sinus over time //

using UnityEngine;

public class Levitate : MonoBehaviour
{
    [SerializeField] private float distance = 1;
    [SerializeField] private float speed = 1;
    private float yPos;

    void OnEnable()
    {
        yPos = transform.position.y;
    }

    void Update()
    {
        float value = Mathf.Sin(Time.time * speed) * distance + yPos;
        transform.position = new Vector3(transform.position.x, value, transform.position.z);
    }
}
