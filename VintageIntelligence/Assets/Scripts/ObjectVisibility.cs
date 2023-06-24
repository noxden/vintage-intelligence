using UnityEngine;
using UnityEngine.Serialization;

public class ObjectVisibility : MonoBehaviour
{
    public GameObject car; // Reference to the static object
    public float visibilityThreshold = 10f; // Distance threshold for object visibility

    private void Start()
    {
        // Find all objects with a Renderer component in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        // Loop through each renderer and set its initial visibility state based on the distance to the static object
        foreach (Renderer renderer in renderers)
        {
            bool isVisible = IsObjectVisible(renderer.gameObject.transform.position);
            SetObjectVisibility(renderer, isVisible);
        }
    }

    private void Update()
    {
        // Find all objects with a Renderer component in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        // Loop through each renderer and update its visibility state based on the distance to the static object
        foreach (Renderer renderer in renderers)
        {
            bool isVisible = IsObjectVisible(renderer.gameObject.transform.position);
            SetObjectVisibility(renderer, isVisible);
        }
    }

    private bool IsObjectVisible(Vector3 objectPosition)
    {
        // Calculate the distance between the object and the car
        float distanceToObject = Vector3.Distance(objectPosition, car.transform.position);

        // Return true if the distance is within the visibility threshold, indicating the object should be visible
        return distanceToObject <= visibilityThreshold;
    }

    private void SetObjectVisibility(Renderer renderer, bool visible)
    {
        renderer.enabled = visible; // Enable or disable the renderer based on the visibility state
    }
}