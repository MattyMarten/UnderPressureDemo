using UnityEngine;
using UnityEngine.InputSystem;

// Turns Johnny's head. He stays in his chair the whole game,
// so this only rotates the camera; it never moves it.
// Two states:
//   No mouse button held: he looks around.
//     The mouse turns his head freely, inside the limits.
//   Left mouse (Grab) or right mouse (RightHand) held: the mouse moves his hand instead.
//     The camera looks at the room's main view and turns a little towards "follow" (his wrist), softly.
// (Build step 2 changes this: free look all the time, and the held object follows the gaze.)
// Put it on the camera, which sits at his eyes.
public class SeatedLook : MonoBehaviour
{
    public float sensitivity = 0.1f;    // degrees per pixel of mouse movement while looking around, not zoomed
    public float maxLeftRight = 70f;    // how far he can turn his head to each side
    public float maxUp = 30f;           // how far he can look up
    public float maxDown = 50f;         // how far he can look down (further, to see the desk)
    public bool canLook = true;         // later: off during the wake-up animation

    public Transform follow;            // what the camera follows softly in the fixed state (the Wrist). Can stay empty.
    [Range(0f, 1f)]
    public float followAmount = 0.3f;   // 0 = never turns to it, 1 = looks straight at it
    public float smoothTime = 0.25f;    // about how many seconds the camera takes to catch up

    // True while no mouse button is held. DeskHand reads it, so the hand stays still meanwhile.
    public bool IsLookingAround { get; private set; }

    InputAction lookAction;
    InputAction grabAction;
    InputAction rightHandAction;
    float startYaw;      // the main view: the camera's direction when the scene starts
    float startPitch;
    float yaw;           // how far he has turned left (-) or right (+) from the main view
    float pitch;         // how far he has looked up (-) or down (+) from the main view
    float yawSpeed;      // SmoothDamp keeps its current speed in these between frames
    float pitchSpeed;
    Camera cam;              // the camera on this object, to know how far CameraZoom has zoomed
    float normalFieldOfView; // its Field of View when the scene starts (not zoomed)

    void Start()
    {
        // "Look" in the Player map of InputSystem_Actions: mouse movement (or the right stick)
        lookAction = InputSystem.actions.FindAction("Player/Look");
        // "Grab" (left mouse) and "RightHand" (right mouse): holding a mouse button
        grabAction = InputSystem.actions.FindAction("Player/Grab");
        rightHandAction = InputSystem.actions.FindAction("Player/RightHand");
        if (grabAction == null || rightHandAction == null)
            Debug.LogError("SeatedLook: the Player map needs the actions \"Grab\" (left mouse) and \"RightHand\" (right mouse). Add them in InputSystem_Actions.");

        // The Field of View you set in the Inspector is "not zoomed"
        cam = GetComponent<Camera>();
        normalFieldOfView = cam.fieldOfView;

        // The rotation you set in the Inspector is the main view
        Vector3 start = transform.localEulerAngles;
        startYaw = start.y;
        startPitch = Mathf.DeltaAngle(0f, start.x);   // 340 → -20, so up and down work around 0

        // Hide the mouse pointer and keep it in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!canLook)
            return;

        // Looking around = neither mouse button is held.
        // "?." means: if the action is missing (null), don't ask it, the answer is just false.
        bool handButtonHeld = grabAction?.IsPressed() == true || rightHandAction?.IsPressed() == true;
        IsLookingAround = !handButtonHeld;

        if (IsLookingAround)
            LookAround();
        else
            FollowSoftly();

        transform.localRotation = Quaternion.Euler(startPitch + pitch, startYaw + yaw, 0f);
    }

    // No hand button held: the mouse turns the head directly
    void LookAround()
    {
        Vector2 delta = lookAction.ReadValue<Vector2>();   // how far the mouse moved this frame

        // Zoomed in, the same mouse movement should turn the head less, or the view jumps.
        // Not zoomed: 60 / 60 = 1. Fully zoomed: 30 / 60 = 0.5, so half as fast.
        float zoomScale = cam.fieldOfView / normalFieldOfView;
        float degreesPerPixel = sensitivity * zoomScale;

        yaw += delta.x * degreesPerPixel;
        pitch -= delta.y * degreesPerPixel;   // mouse up means look up, which is a smaller x angle

        yaw = Mathf.Clamp(yaw, -maxLeftRight, maxLeftRight);
        pitch = Mathf.Clamp(pitch, -maxUp, maxDown);

        // No old speed left over, so following the hand starts calmly when a hand button is pressed
        yawSpeed = 0f;
        pitchSpeed = 0f;
    }

    // A hand button held: turn softly towards the main view, a little towards "follow"
    void FollowSoftly()
    {
        float targetYaw = 0f;     // 0, 0 = straight at the main view
        float targetPitch = 0f;

        if (follow != null)
        {
            // The direction from his eyes to the hand, as angles
            Vector3 direction = follow.position - transform.position;
            float flat = new Vector2(direction.x, direction.z).magnitude;   // the distance along the floor
            float yawToIt = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float pitchToIt = -Mathf.Atan2(direction.y, flat) * Mathf.Rad2Deg;   // below him → positive (down)

            // Measured from the main view, then only a part of it
            targetYaw = Mathf.DeltaAngle(startYaw, yawToIt) * followAmount;
            targetPitch = Mathf.DeltaAngle(startPitch, pitchToIt) * followAmount;
        }

        targetYaw = Mathf.Clamp(targetYaw, -maxLeftRight, maxLeftRight);
        targetPitch = Mathf.Clamp(targetPitch, -maxUp, maxDown);

        // Move a step towards the target: fast when far away, slower when close
        yaw = Mathf.SmoothDamp(yaw, targetYaw, ref yawSpeed, smoothTime);
        pitch = Mathf.SmoothDamp(pitch, targetPitch, ref pitchSpeed, smoothTime);
    }
}