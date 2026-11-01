using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Lets his right hand hold a stamp. While D (GripRight) is held, the stamp under the hand
// glides into the hand. When D is let go, it glides back to its own spot.
// The grip works even when no mouse button is held, so he can keep holding while he looks around.
// (Rework step 4 makes this one per hand: A for the left hand, D for the right.)
// Put it on HandArea, next to DeskHand.
public class StampHolder : MonoBehaviour
{
    public StampPress press;                         // a pressed-down stamp can't be let go
    public Transform grip;                           // the point in his palm where a stamp sits
    public List<Stamp> stamps = new List<Stamp>();   // GreenStamp and RedStamp
    public float reach = 0.1f;                       // how close (meters, seen from above) the hand must be to a stamp

    // The stamp in his hand now, or null. Stamping (a later step) will read it.
    public Stamp Held { get; private set; }

    InputAction gripAction;

    void Start()
    {
        // "GripRight" in the Player map of InputSystem_Actions: D
        gripAction = InputSystem.actions.FindAction("Player/GripRight");
        if (gripAction == null)
            Debug.LogError("StampHolder: there is no action \"GripRight\" in the Player map. Add it in InputSystem_Actions, bound to D.");
    }

    void Update()
    {
        if (gripAction == null)
            return;

        // D was just pressed and the hand is empty
        if (Held == null && gripAction.WasPressedThisFrame())
        {
            Stamp nearest = FindNearest();
            if (nearest != null)
            {
                Held = nearest;
                Held.GoToHand(grip);
            }
        }

        // D is not held any more: the stamp goes home.
        // But not while it is pressed down: then it goes home after Space is let go.
        if (Held != null && !gripAction.IsPressed() && !press.IsPressing)
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