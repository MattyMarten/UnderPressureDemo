using UnityEngine;
using UnityEngine.InputSystem;

// Lets the player turn Johnny's head with the mouse. He stays in his chair the whole game,
// so this only rotates the camera; it never moves it.
// Put it on the camera, which sits at his eyes.
public class SeatedLook : MonoBehaviour
{
    public float sensitivity = 0.1f;    // degrees per pixel of mouse movement
    public float maxLeftRight = 70f;    // how far he can turn his head to each side
    public float maxUp = 30f;           // how far he can look up
    public float maxDown = 50f;         // how far he can look down (further, to see the desk)
    public bool canLook = true;         // later: off during the wake-up animation

    InputAction lookAction;
    float startYaw;      // the camera's direction when the scene starts
    float startPitch;
    float yaw;           // how far he has turned left (-) or right (+) from there
    float pitch;         // how far he has looked up (-) or down (+) from there

    void Start()
    {
        // "Look" in the Player map of InputSystem_Actions: mouse movement (or the right stick)
        lookAction = InputSystem.actions.FindAction("Player/Look");

        // The rotation you set in the Inspector is "straight ahead" for him
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

        yaw += delta.x * sensitivity;
        pitch -= delta.y * sensitivity;   // mouse up means look up, which is a smaller x angle

        yaw = Mathf.Clamp(yaw, -maxLeftRight, maxLeftRight);
        pitch = Mathf.Clamp(pitch, -maxUp, maxDown);

        transform.localRotation = Quaternion.Euler(startPitch + pitch, startYaw + yaw, 0f);
    }
}
