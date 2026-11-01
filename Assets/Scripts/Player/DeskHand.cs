using UnityEngine;
using UnityEngine.InputSystem;

// His right arm in the fixed state. The mouse moves his wrist around inside a flat area on the desk.
// Only the Wrist is kept inside the area. His hand is its child, so it follows it,
// and the hand (with the stamp it holds) can reach a little past the area.
// ArmPose builds the forearm and upper arm from the wrist's position.
// While Shift is held he looks around instead, and the wrist stays where it is.
// While a stamp is pressed (StampPress), the wrist can't move sideways and is pushed down.
// Put it on the HandArea object, which lies on the desk surface. The Wrist is its child.
public class DeskHand : MonoBehaviour
{
    public SeatedLook look;            // to know when he is looking around (Shift)
    public StampPress press;           // to know when a stamp is pressed down, and how far
    public Transform wrist;            // the Wrist child: the point that moves

    public Vector2 areaSize = new Vector2(1.2f, 0.6f);   // width (left-right) and depth (near-far), in meters
    public float hoverHeight = 0.15f;  // how high above the desk his wrist floats
    public float sensitivity = 0.001f; // meters per pixel of mouse movement
    public bool canMove = true;        // later: off until his right hand is freed at the end of the intro

    InputAction moveAction;
    Vector2 offset;   // where the wrist is, from the middle of the area: x = right, y = away from him

    void Start()
    {
        // The same mouse movement that SeatedLook uses. Only one of them uses it at a time.
        moveAction = InputSystem.actions.FindAction("Player/Look");
    }

    void Update()
    {
        if (canMove && !look.IsLookingAround && !press.IsPressing)
        {
            Vector2 delta = moveAction.ReadValue<Vector2>();   // how far the mouse moved this frame
            offset += delta * sensitivity;                      // mouse up → wrist moves away from him

            // Keep the wrist inside the area
            float halfWidth = areaSize.x / 2f;
            float halfDepth = areaSize.y / 2f;
            offset.x = Mathf.Clamp(offset.x, -halfWidth, halfWidth);
            offset.y = Mathf.Clamp(offset.y, -halfDepth, halfDepth);
        }

        // Local position: measured from HandArea, along HandArea's own axes.
        // press.Depth is 0 except while stamping: then the wrist goes down so the stamp touches.
        wrist.localPosition = new Vector3(offset.x, hoverHeight - press.Depth, offset.y);
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