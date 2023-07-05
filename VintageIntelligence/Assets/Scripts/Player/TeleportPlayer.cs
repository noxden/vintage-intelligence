// Created 05.07.2023 by Krista Plagemann//
// Teleports the player to a given position. //

using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    /// <summary>
    /// Teleports the player to this position.
    /// </summary>
    /// <param name="targetTransform">Position to teleport to.</param>
    public void TeleportTo(Transform targetTransform)
    {
        // change our position to the objects position, but the height should be 0 always
        transform.position = new Vector3(targetTransform.position.x, 0.0f, targetTransform.position.z);
        // turn so we are looking forward
        transform.forward = targetTransform.forward; 
    }

    /// <summary>
    /// Teleports the player to this position.
    /// </summary>
    /// <param name="targetTransform">Position to teleport to.</param>
    /// <param name="lookRotation">Direction to look. (forward direction).</param>
    public void TeleportTo(Transform targetTransform, Vector3 lookRotation)
    {
        transform.position = new Vector3(targetTransform.position.x, 0.0f, targetTransform.position.z);
        // turn in target direction
        transform.forward = lookRotation;
    }
}
