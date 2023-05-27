

using UnityEngine;

public class PickTest : MonoBehaviour
{
    public Pickupable ThisPickupable;

    private bool pickedup = false;
    public bool discardObject = false;

    // Update is called once per frame
    void Update()
    {
        if(discardObject)
        {
            ThisPickupable.Discard();
            discardObject = false;
        }

        if(!pickedup)
        if (transform.position.y >= 2)
        {
                pickedup = true;
            ThisPickupable.PickThisUp();
        }
    }

    public void RevertPickup()
    {
        pickedup=false;
    }
}
