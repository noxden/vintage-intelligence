//Taken from other project, created by Sebastian Kostur, 13.04.2022//

using System.Collections;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public float Yoffset;
    private bool followPlayerTemp = false;
    public bool followPlayer = true;
    public bool YPlaneOnly;
    public bool interpolate;
    public float interpolationspeed = 1;
    [Tooltip("How fast the object should return to initial position after player is out of range. Set to 0 to disable")]
    public float returnspeed = 0.5f;
    [Tooltip("In Degrees towards left or right direction")]
    public float visiblerange = 90;

    private Camera playerCamera;
    private Quaternion rotation;
    private Quaternion startLocalRotation;
    private bool resetLook;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        startLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = playerCamera.transform.position - transform.position; //get the direction vector from the transform to the player
        float angle = Vector3.Angle(transform.parent.forward, direction); //get the angle towards the player

        if (!resetLook)
        {
            if (angle <= visiblerange && (followPlayer || followPlayerTemp)) //if enabled and if within the range
            {
                if (YPlaneOnly) direction.y = 0; //zero out Y plane if enabled

                Quaternion lookatrotation = Quaternion.LookRotation(direction); //get the look at rotation
                lookatrotation *= Quaternion.Euler(0, Yoffset, 0); //offset rotation

                if (interpolate)
                    rotation = Quaternion.Lerp(transform.rotation, lookatrotation, Time.deltaTime * interpolationspeed);
                else
                    rotation = lookatrotation;
            }
            else if (returnspeed != 0) //return to default position
            {
                rotation = Quaternion.Lerp(transform.rotation, transform.parent.rotation, Time.deltaTime * returnspeed);
            }
        }
        else
        {
            rotation = transform.parent.rotation;
            resetLook = false;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = rotation;
    }

    public void LookAtTemporarily(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(lookAtTemporarily(duration));
    }

    IEnumerator lookAtTemporarily(float duration)
    {
        followPlayerTemp = true;
        yield return new WaitForSeconds(duration);
        followPlayerTemp = false;
    }

    public void LookAt()
    {
        StopAllCoroutines();
        followPlayer = true;
    }

    public void ResetLook()
    {
        StopAllCoroutines();
        transform.localRotation = startLocalRotation;
        rotation = transform.rotation;
        resetLook = true;
    }
}