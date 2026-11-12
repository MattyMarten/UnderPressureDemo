using UnityEngine;

// Something Johnny can pick up: a stamp now, the rope and other things later.
// The object decides how it moves. The Grabber (on the camera) only tells it
// "you are picked up", "here is where he looks" and "you are let go".
// While held, it floats a little above the desk, where his gaze hits the desk, inside its zone.
// When let go, it glides back to the spot where it stood when the scene started.
// It always keeps its own rotation, so a stamp always stays straight.
// When picked up it first waits for his hand to reach it (DeskHand calls HandArrived).
// From then on it sits in Johnny's real hand: its grip point is put on the hand's RealGrip every frame.
// His gaze only decides where the hand *tries* to go (the wanted point). If his arm can't reach that far,
// the object stays in his hand anyway, so it can never leave the hand.
// Another script can lock it (StampPress does, while the stamp is pressed down):
// then it stops following his gaze and can't be let go until it is unlocked.
// Put it on each object that can be held (GreenStamp, RedStamp). It needs a collider on it or on a child.
public class Grabbable : MonoBehaviour
{
    public GrabZone zone;               // the box it can move in while held
    public Transform gripPoint;         // where his hand holds it. Empty = the object's own position (right for the stamps)
    public float hoverHeight = 0.05f;   // how high above the desk it floats while held, in meters
    public float followTime = 0.08f;    // about how many seconds it takes to catch up with his gaze
    public float returnTime = 0.15f;    // about how many seconds it takes to glide home

    public bool IsHeld { get; private set; }
    public bool IsHome { get; private set; } = true;   // standing still on its own spot
    public bool InHand { get; private set; }           // held, and the hand has reached it

    // While true: it stays where it is (doesn't follow his gaze) and the Grabber won't let it go.
    // Set by the script that uses the object (StampPress while pressing). Anyone may set it, so "set" is public.
    public bool Locked { get; set; }

    // Where the hand should try to put the grip point now. DeskHand reads it while the object is in the hand.
    // (The grip point is always the same distance from the object's own position, because it never turns.)
    public Vector3 WantedGripPosition => wanted + (GripPoint.position - transform.position);

    // The point the hand goes to: gripPoint if one is set, otherwise the object itself
    public Transform GripPoint => gripPoint != null ? gripPoint : transform;

    Vector3 homePosition;   // where it stood when the scene started
    Vector3 target;         // where his gaze says it should be (inside the zone, above the desk)
    Vector3 wanted;         // follows target smoothly: where the hand tries to bring it
    Vector3 velocity;       // SmoothDamp keeps its current speed here between frames
    Transform holdingGrip;  // the RealGrip on Johnny's hand while it is in the hand, else null

    void Awake()
    {
        homePosition = transform.position;
    }

    // The Grabber calls this when he presses left mouse while looking at it
    public void PickUp()
    {
        IsHeld = true;
        InHand = false;   // the hand still has to reach it
        IsHome = false;
        target = transform.position;   // until the first Follow, stay where it is
    }

    // DeskHand calls this when the hand has reached it, with the point in the real hand that holds it
    public void HandArrived(Transform grip)
    {
        InHand = true;
        holdingGrip = grip;
        wanted = transform.position;   // the hand starts from where the object is now
        velocity = Vector3.zero;
    }

    // The Grabber calls this every frame while it is held, with the line he is looking along
    public void Follow(Ray gaze)
    {
        // Where does his gaze hit the desk surface?
        // Raycast gives back false if it never hits (for example when he looks up at the ceiling).
        // Then the target stays where it was, and the object waits there.
        if (zone.Floor.Raycast(gaze, out float distance))
        {
            Vector3 onDesk = zone.Clamp(gaze.GetPoint(distance));    // the point, kept inside the zone
            target = onDesk + zone.transform.up * hoverHeight;        // a little above the desk
        }
    }

    // The Grabber calls this when he lets go of left mouse
    public void Release()
    {
        IsHeld = false;
        InHand = false;
        Locked = false;   // to be safe: a let-go object is never locked
        holdingGrip = null;
        velocity = Vector3.zero;   // glide home from rest, not with the hand's old speed
    }

    void Update()
    {
        if (IsHeld && !InHand)
        {
            // Picked up, but the hand isn't there yet: stay put and wait for it
        }
        else if (IsHeld && Locked)
        {
            // In the hand but locked (pressed down): the wanted point stays where it is.
            // velocity = zero, so when it is unlocked it starts following again from rest, without a jump.
            velocity = Vector3.zero;
        }
        else if (IsHeld)
        {
            // In the hand: only the wanted point moves, smoothly towards where he looks.
            // The object itself is moved in LateUpdate, onto the hand.
            wanted = Vector3.SmoothDamp(wanted, target, ref velocity, followTime);
        }
        else if (!IsHome)
        {
            // Let go: glide home
            transform.position = Vector3.SmoothDamp(transform.position, homePosition, ref velocity, returnTime);

            // Close enough (1 mm): put it exactly on its spot and stop moving it
            if (Vector3.Distance(transform.position, homePosition) < 0.001f)
            {
                transform.position = homePosition;
                velocity = Vector3.zero;
                IsHome = true;
            }
        }
    }

    // LateUpdate runs after the animation and the IK, so the real hand is in its final place for this frame.
    // Move the object so its grip point sits exactly on the hand's RealGrip.
    void LateUpdate()
    {
        if (InHand && holdingGrip != null)
            transform.position += holdingGrip.position - GripPoint.position;
    }
}