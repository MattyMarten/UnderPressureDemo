using UnityEngine;
using UnityEngine.InputSystem;

// Zooms Johnny's view while W is held.
// It only changes the camera's Field of View (like binoculars), so the camera never moves:
// he stays in his chair. Letting go of W zooms back out.
// Zoom works all the time: while looking around and while holding a mouse button.
// SeatedLook reads the Field of View and turns the head slower while zoomed.
// Put it on the camera, next to SeatedLook.
public class CameraZoom : MonoBehaviour
{
    public float zoomedFieldOfView = 30f;  // degrees when fully zoomed. Smaller = closer.
    public float smoothTime = 0.15f;       // about how many seconds zooming in or out takes
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

        // Zoomed while W is held (and zooming is allowed right now)
        bool zoomed = canZoom && zoomAction.IsPressed();

        float target = zoomed ? zoomedFieldOfView : normalFieldOfView;

        // Move a step towards the target each frame: fast when far away, slower when close
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, target, ref fovSpeed, smoothTime);
    }
}