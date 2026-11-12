using UnityEngine;
using UnityEngine.Animations.Rigging;

// Johnny leans forward when his real arm can't reach what he is reaching for or holding.
// Only as much as needed, and back again when it is close enough or let go.
// The lean bends his spine (an Override Transform constraint in ArmRig, above RightArmIK,
// so the spine bends first and the arm reaches from the new shoulder position)
// and moves the camera (his eyes) forward and down with it.
// Put it on ArmRig.

// Runs after DeskHand (100), so the hand target has already moved this frame
[DefaultExecutionOrder(200)]
public class ReachLean : MonoBehaviour
{
    public Grabber grabber;               // to know if he is reaching for or holding something (Main Camera)
    public OverrideTransform spineLean;   // the SpineLean constraint. Its weight = how much he leans: 0 none, 1 full.

    [Header("His real arm (the same bones as in RightArmIK)")]
    public Transform shoulder;            // RightArmIK's Root (upper arm bone)
    public Transform elbow;               // RightArmIK's Mid (forearm bone)
    public Transform hand;                // RightArmIK's Tip (hand bone)
    public Transform target;              // RealHandTarget: where the hand wants to be

    [Header("Leaning")]
    [Range(0.8f, 1f)]
    public float comfortableReach = 0.95f;   // part of the full arm length he uses before he starts to lean
    public float maxLeanDistance = 0.2f;     // how far (meters) his shoulder moves forward at full lean. Tune it.
    public float leanTime = 0.25f;           // about how many seconds a lean takes

    [Header("Camera")]
    public Transform cameraToMove;           // Main Camera
    public Vector3 cameraAtFullLean = new Vector3(0f, -0.05f, 0.15f);   // how far the camera moves at full lean

    float armLength;        // upper arm + forearm, measured once at the start
    float lean;             // 0 to 1, now
    float leanSpeed;        // SmoothDamp keeps its current speed here between frames
    Vector3 cameraStart;    // the camera's position when not leaning

    void Start()
    {
        armLength = Vector3.Distance(shoulder.position, elbow.position)
                  + Vector3.Distance(elbow.position, hand.position);
        cameraStart = cameraToMove.localPosition;
    }

    void Update()
    {
        float wanted = 0f;   // nothing held: no lean

        if (grabber.Held != null)
        {
            // How far the target is from his shoulder now
            float distance = Vector3.Distance(shoulder.position, target.position);

            // The shoulder has already moved forward by the lean we have now.
            // Add that back, to get the distance as if he sat straight.
            // (A simple guess: it assumes the lean moves the shoulder straight towards the target.)
            float distanceSittingStraight = distance + lean * maxLeanDistance;

            // How much further than comfortable that is, as a part of the full lean (0 to 1)
            float tooFar = distanceSittingStraight - armLength * comfortableReach;
            wanted = Mathf.Clamp01(tooFar / maxLeanDistance);
        }

        // Smoothly towards the wanted lean
        lean = Mathf.SmoothDamp(lean, wanted, ref leanSpeed, leanTime);

        // The spine: the constraint blends from his normal pose (0) to fully bent (1)
        spineLean.weight = lean;

        // The camera: the same part of its full lean
        cameraToMove.localPosition = cameraStart + cameraAtFullLean * lean;
    }
}
