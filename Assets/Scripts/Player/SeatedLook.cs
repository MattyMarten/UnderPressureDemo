using UnityEngine;
using UnityEngine.InputSystem;

// Turns Johnny's head. He stays in his chair the whole game,
// so this only rotates the camera; it never moves it.
// The mouse always turns his head, inside the limits. No button is needed.
// While zoomed, the same mouse movement turns the head less.
// While he holds an object, the middle of the screen stays over the object's zone:
// he can look around inside it, but not past its edges.
// Put it on the camera, which sits at his eyes.
public class SeatedLook : MonoBehaviour
{
    public float sensitivity = 0.1f;    // degrees per pixel of mouse movement, not zoomed
    public float maxLeftRight = 70f;    // how far he can turn his head to each side
    public float maxUp = 30f;           // how far he can look up
    public float maxDown = 50f;         // how far he can look down (further, to see the desk)
    public bool canLook = true;         // later: off during parts of the wake-up

    InputAction lookAction;
    float startYaw;          // the main view: the camera's direction when the scene starts
    float startPitch;
    float yaw;               // how far he has turned left (-) or right (+) from the main view
    float pitch;             // how far he has looked up (-) or down (+) from the main view
    Camera cam;              // the camera on this object, to know how far CameraZoom has zoomed
    Grabber grabber;         // the Grabber on this object, to know if he holds something
    float normalFieldOfView; // its Field of View when the scene starts (not zoomed)

    void Start()
    {
        // "Look" in the Player map of InputSystem_Actions: mouse movement (or the right stick)
        lookAction = InputSystem.actions.FindAction("Player/Look");

        // The Field of View you set in the Inspector is "not zoomed"
        cam = GetComponent<Camera>();
        normalFieldOfView = cam.fieldOfView;

        // Can be missing: then the view is never limited
        grabber = GetComponent<Grabber>();

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

        Vector2 delta = lookAction.ReadValue<Vector2>();   // how far the mouse moved this frame

        // Where he looked last frame, in case the new direction isn't allowed
        float lastYaw = yaw;
        float lastPitch = pitch;

        // Zoomed in, the same mouse movement should turn the head less, or the view jumps.
        // Not zoomed: 60 / 60 = 1. Fully zoomed: 30 / 60 = 0.5, so half as fast.
        float zoomScale = cam.fieldOfView / normalFieldOfView;
        float degreesPerPixel = sensitivity * zoomScale;

        yaw += delta.x * degreesPerPixel;
        pitch -= delta.y * degreesPerPixel;   // mouse up means look up, which is a smaller x angle

        yaw = Mathf.Clamp(yaw, -maxLeftRight, maxLeftRight);
        pitch = Mathf.Clamp(pitch, -maxUp, maxDown);

        if (grabber != null && grabber.Held != null)
            KeepGazeInZone(grabber.Held.zone, lastYaw, lastPitch);

        transform.localRotation = Quaternion.Euler(startPitch + pitch, startYaw + yaw, 0f);
    }

    // While he holds something, the middle of the screen must stay over the object's zone.
    // If the new yaw and pitch look past the zone, turn the head back to the nearest point inside it.
    void KeepGazeInZone(GrabZone zone, float lastYaw, float lastPitch)
    {
        // The line he would look along with the new yaw and pitch
        Ray gaze = new Ray(transform.position, GazeDirection(yaw, pitch));

        // Looking above the desk: the gaze never hits it. Keep last frame's direction.
        if (!zone.Floor.Raycast(gaze, out float distance))
        {
            yaw = lastYaw;
            pitch = lastPitch;
            return;
        }

        Vector3 onDesk = gaze.GetPoint(distance);   // where he would look on the desk
        Vector3 inZone = zone.Clamp(onDesk);        // the nearest point inside the zone

        // Inside the zone already: Clamp didn't move it, so nothing to do
        if (Vector3.Distance(onDesk, inZone) < 0.0001f)
            return;

        // Outside: work out the yaw and pitch that look straight at the point inside the zone.
        // The same angles as the old "follow" code: Atan2 turns a direction into an angle.
        Vector3 direction = inZone - transform.position;
        if (transform.parent != null)
            direction = transform.parent.InverseTransformDirection(direction);   // into the same space as yaw and pitch

        float flat = new Vector2(direction.x, direction.z).magnitude;   // the distance along the floor
        yaw = Mathf.DeltaAngle(startYaw, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
        pitch = Mathf.DeltaAngle(startPitch, -Mathf.Atan2(direction.y, flat) * Mathf.Rad2Deg);   // below him → positive (down)

        // The head limits still win
        yaw = Mathf.Clamp(yaw, -maxLeftRight, maxLeftRight);
        pitch = Mathf.Clamp(pitch, -maxUp, maxDown);
    }

    // The direction he would look in with this yaw and pitch, in the world
    Vector3 GazeDirection(float yawToTest, float pitchToTest)
    {
        Quaternion local = Quaternion.Euler(startPitch + pitchToTest, startYaw + yawToTest, 0f);
        Quaternion world = transform.parent != null ? transform.parent.rotation * local : local;
        return world * Vector3.forward;   // a rotation times "forward" = the direction it points
    }
}