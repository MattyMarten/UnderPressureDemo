using UnityEngine;
using UnityEngine.InputSystem;

// Johnny's "picking up", from his point of view.
// Look at an object (the middle of the screen) and press left mouse (Grab): if it has a Grabbable, it is picked up.
// While left mouse is held, it is told every frame where he is looking.
// Let go of left mouse: it is let go (but not while it is Locked: then as soon as it is unlocked).
// How the object moves is decided by the object itself (Grabbable), not here.
// Put it on the camera.
public class Grabber : MonoBehaviour
{
    public float reach = 1.5f;     // how far away (meters) something can be and still be picked up
    public bool canGrab = true;    // later: off until his right hand is freed

    // The object in his hand now, or null. Other scripts (stamping, the hand) will read it.
    public Grabbable Held { get; private set; }

    InputAction grabAction;

    void Start()
    {
        // "Grab" in the Player map of InputSystem_Actions: left mouse
        grabAction = InputSystem.actions.FindAction("Player/Grab");
        if (grabAction == null)
            Debug.LogError("Grabber: there is no action \"Grab\" in the Player map. Add it in InputSystem_Actions, bound to left mouse.");
    }

    void Update()
    {
        if (grabAction == null)
            return;

        // The line he looks along: from his eyes, straight out of the middle of the screen
        Ray gaze = new Ray(transform.position, transform.forward);

        // Left mouse just pressed, hand empty: is he looking at something he can pick up?
        if (Held == null && canGrab && grabAction.WasPressedThisFrame())
        {
            // The first collider the gaze hits, if any, within reach
            if (Physics.Raycast(gaze, out RaycastHit hit, reach))
            {
                // The collider may be on a child (the stamp's Base or Handle), so look upwards for the Grabbable
                Grabbable grabbable = hit.collider.GetComponentInParent<Grabbable>();
                if (grabbable != null)
                {
                    Held = grabbable;
                    Held.PickUp();
                }
            }
        }

        if (Held != null)
        {
            if (grabAction.IsPressed())
                Held.Follow(gaze);   // still holding: tell it where he looks
            else if (!Held.Locked)
            {
                Held.Release();      // let go
                Held = null;
            }
            // else: left mouse is up, but the object is locked (a stamp pressed down).
            // Keep holding it. The first frame after it is unlocked, this lets it go.
        }
    }
}