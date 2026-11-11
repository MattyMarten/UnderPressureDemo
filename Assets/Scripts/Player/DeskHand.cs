using UnityEngine;

// His right hand. It no longer follows the mouse: it follows what he holds.
// Picking up: the hand reaches for the object. When it gets there, it tells the object (HandArrived),
// and from then on the hand moves exactly with it, so its Grip always sits on the object's grip point.
// Nothing held: the hand slowly goes back to where it rests (where the Wrist is placed in the editor).
// Only the Wrist is moved; the Hand is its child. ArmPose and the real arm (IK) follow it.
// The yellow area is how far the wrist can go, which is his reach for now.
// (Step 5 adds leaning forward when the object goes further than that.)
// Put it on the HandArea object, which lies on the desk surface. The Wrist is its child.

// Runs after the normal scripts (they are 0, this is 100), so the held object
// has already moved this frame when the hand follows it. Otherwise the hand would lag one frame behind.
[DefaultExecutionOrder(100)]
public class DeskHand : MonoBehaviour
{
    public Grabber grabber;            // to know what he holds (on Main Camera)
    public Transform wrist;            // the Wrist child: the point that moves
    public Transform grip;             // the Grip under the Hand: the point that sits on the object's grip point

    public Vector2 areaSize = new Vector2(1.2f, 0.6f);   // how far the wrist can go: width and depth, in meters
    public float reachTime = 0.15f;    // about how many seconds the hand takes to reach an object
    public float returnTime = 0.5f;    // about how many seconds it takes to go back to rest (slower)
    public bool canMove = true;        // later: off until his right hand is freed

    Vector3 restPosition;   // the Wrist's position as placed in the editor, measured inside HandArea
    Vector3 velocity;       // SmoothDamp keeps its current speed here between frames

    void Start()
    {
        restPosition = wrist.localPosition;
    }

    void Update()
    {
        Grabbable held = grabber.Held;

        // Nothing held (or the hand can't move): back to rest, slowly
        if (!canMove || held == null)
        {
            wrist.localPosition = Vector3.SmoothDamp(wrist.localPosition, restPosition, ref velocity, returnTime);
            return;
        }

        Vector3 target = WristTargetFor(held);

        if (held.InHand)
        {
            // Holding it: exactly with the object, no smoothing, so they never drift apart
            wrist.localPosition = target;
            velocity = Vector3.zero;
        }
        else
        {
            // Still reaching for it
            wrist.localPosition = Vector3.SmoothDamp(wrist.localPosition, target, ref velocity, reachTime);

            // Close enough (1 cm): the hand has it
            if (Vector3.Distance(wrist.localPosition, target) < 0.01f)
                held.HandArrived();
        }
    }

    // Where the Wrist must be so that the Grip lands on the object's grip point.
    // The Grip is always the same distance from the Wrist (the hand doesn't turn),
    // so: wrist = grip point - (grip - wrist).
    Vector3 WristTargetFor(Grabbable held)
    {
        Vector3 gripFromWrist = grip.position - wrist.position;
        Vector3 wristInWorld = held.GripPoint.position - gripFromWrist;

        // Into HandArea's own space, and kept inside the area (his reach)
        Vector3 local = transform.InverseTransformPoint(wristInWorld);
        local.x = Mathf.Clamp(local.x, -areaSize.x / 2f, areaSize.x / 2f);
        local.z = Mathf.Clamp(local.z, -areaSize.y / 2f, areaSize.y / 2f);
        return local;
    }

    // Draws the area as a yellow box on the desk in the Scene view (not in the game),
    // so you can see and size it while you place HandArea
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, 0f, areaSize.y));
    }
}