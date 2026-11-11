using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Lets his right hand hold a stamp. While left mouse (Grab) is held, the hand moves,
// and as soon as it is close to a stamp, that stamp glides into the hand.
// When left mouse is let go, the stamp glides back to its own spot.
// (Temporary. Build step 4 replaces this script with Grabbable:
//  look at an object and hold left mouse to pick it up.)
// Put it on HandArea, next to DeskHand.
public class StampHolder : MonoBehaviour
{
    public StampPress press;                         // a pressed-down stamp can't be let go
    public Transform grip;                           // the point in his palm where a stamp sits
    public List<Stamp> stamps = new List<Stamp>();   // GreenStamp and RedStamp
    public float reach = 0.1f;                       // how close (meters, seen from above) the hand must be to a stamp

    // The stamp in his hand now, or null. Stamping (a later step) will read it.
    public Stamp Held { get; private set; }

    InputAction grabAction;

    void Start()
    {
        // "Grab" in the Player map of InputSystem_Actions: left mouse
        grabAction = InputSystem.actions.FindAction("Player/Grab");
        if (grabAction == null)
            Debug.LogError("StampHolder: there is no action \"Grab\" in the Player map. Add it in InputSystem_Actions, bound to left mouse.");
    }

    void Update()
    {
        if (grabAction == null)
            return;

        // Left mouse is held and the hand is empty: grab a stamp as soon as one is within reach.
        // (IsPressed, not WasPressedThisFrame: the hand only moves while the button is held,
        //  so it reaches the stamp after the press, not at the moment of it.)
        if (Held == null && grabAction.IsPressed())
        {
            Stamp nearest = FindNearest();
            if (nearest != null)
            {
                Held = nearest;
                Held.GoToHand(grip);
            }
        }

        // Left mouse is not held any more: the stamp goes home.
        // But not while it is pressed down: then it goes home after Space is let go.
        if (Held != null && !grabAction.IsPressed() && !press.IsPressing)
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