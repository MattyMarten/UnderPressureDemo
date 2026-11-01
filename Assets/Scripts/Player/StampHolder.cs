using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Lets his right hand hold a stamp. While the pickup button is held, the stamp under the hand
// glides into the hand. When the button is let go, it glides back to its own spot.
// Put it on HandArea, next to DeskHand.
public class StampHolder : MonoBehaviour
{
    public SeatedLook look;                          // no picking up while he looks around (Shift)
    public StampPress press;                         // a pressed-down stamp can't be let go
    public Transform grip;                           // the point in his palm where a stamp sits
    public List<Stamp> stamps = new List<Stamp>();   // GreenStamp and RedStamp
    public float reach = 0.1f;                       // how close (meters, seen from above) the hand must be to a stamp

    // The stamp in his hand now, or null. Stamping (a later step) will read it.
    public Stamp Held { get; private set; }

    InputAction pickupAction;

    void Start()
    {
        pickupAction = InputSystem.actions.FindAction("Player/Pickup");
        if (pickupAction == null)
            Debug.LogError("StampHolder: there is no action \"Pickup\" in the Player map. Add it in InputSystem_Actions.");
    }

    void Update()
    {
        if (pickupAction == null)
            return;

        // The button was just pressed, the hand is empty, and he is not looking around
        if (Held == null && pickupAction.WasPressedThisFrame() && !look.IsLookingAround)
        {
            Stamp nearest = FindNearest();
            if (nearest != null)
            {
                Held = nearest;
                Held.GoToHand(grip);
            }
        }

        // The button is not held any more: the stamp goes home.
        // But not while it is pressed down: then it goes home after Space is let go.
        if (Held != null && !pickupAction.IsPressed() && !press.IsPressing)
        {
            Held.GoHome();
            Held = null;
        }
    }

    // The stamp closest to the grip, seen from above, if one is within reach. Else null.
    Stamp FindNearest()
    {
        Stamp nearest = null;
        float nearestDistance = reach;   // anything further than reach doesn't count

        foreach (Stamp stamp in stamps)
        {
            Vector3 a = grip.position;
            Vector3 b = stamp.transform.position;
            float distance = Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));   // height ignored

            if (distance <= nearestDistance)
            {
                nearest = stamp;
                nearestDistance = distance;
            }
        }
        return nearest;
    }
}