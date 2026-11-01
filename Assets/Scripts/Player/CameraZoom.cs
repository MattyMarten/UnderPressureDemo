using UnityEngine;
using UnityEngine.InputSystem;

// Zooms Johnny's view while W is held.
// It only changes the camera's Field of View (like binoculars), so the camera never moves:
// he stays in his chair. Letting go of W zooms back out.
// By default he can only zoom while looking around (no mouse button held),
// so pressing a mouse button to move a hand also zooms out.
// Tick "Zoom Without Shift" to let him zoom while moving a hand too.
// Put it on the camera, next to SeatedLook.
public class CameraZoom : MonoBehaviour
{
    public SeatedLook look;                // to know when he is looking around (no mouse button held)
    public float zoomedFieldOfView = 30f;  // degrees when fully zoomed. Smaller = closer.
    public float smoothTime = 0.15f;       // about how many seconds zooming in or out takes
    public bool zoomWithoutShift = false;  // false = zoom only while looking around. Old name, kept so the Inspector value stays.
    public bool canZoom = true;            // later: off during the wake-up animation

    Camera cam;
    InputAction zoomAction;
    float normalFieldOfView;   // the camera's Field of View when the scene starts
    float fovSpeed;            // SmoothDamp keeps its current speed here between frames

    void Start()
    {
        cam = GetComponent<Camera>();
        normalFieldOfView = cam.fieldOfView;   // the value you set in the Inspector (60 now)

        // "Zoom" in the Player map of InputSystem_Actions: W
        zoomAction = InputSystem.actions.FindAction("Player/Zoom");
        if (zoomAction == null)
            Debug.LogError("CameraZoom: there is no action \"Zoom\" in the Player map. Add it in InputSystem_Actions, bound to W.");
    }

    void Update()
    {
        if (zoomAction == null)
            return;

        // He zooms only while W is held, and only while looking around,
        // unless "Zoom Without Shift" is ticked
        bool allowedNow = look.IsLookingAround || zoomWithoutShift;
        bool zoomed = canZoom && allowedNow && zoomAction.IsPressed();

        float target = zoomed ? zoomedFieldOfView : normalFieldOfView;

        // Move a step towards the target each frame: fast when far away, slower when close
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, target, ref fovSpeed, smoothTime);
    }
}