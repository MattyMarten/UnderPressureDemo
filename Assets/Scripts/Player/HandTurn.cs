using UnityEngine;
using UnityEngine.InputSystem;

// Turns his hand at the wrist: hold Q to turn left, E to turn right.
// The Hand turns around its own pivot, which sits on the wrist,
// so the palm and a held stamp swing around the wrist. The forearm doesn't turn.
// While Shift is held he only looks around, so the hand doesn't turn.
// While a stamp is pressed down it doesn't turn either.
// Put it on HandArea, next to DeskHand.
public class HandTurn : MonoBehaviour
{
    public SeatedLook look;            // to know when he is looking around (Shift)
    public StampPress press;           // to know when a stamp is pressed down
    public Transform hand;             // the Hand child of the Wrist
    public float maxLeft = 80f;        // degrees he can turn it to the left
    public float maxRight = 40f;       // degrees he can turn it to the right
    public float turnSpeed = 120f;     // degrees per second while Q or E is held
    public bool canTurn = true;        // later: off until his right hand is freed at the end of the intro

    InputAction turnAction;
    float angle;   // how far the hand is turned now: left is negative, right is positive

    void Start()
    {
        turnAction = InputSystem.actions.FindAction("Player/TurnHand");
        if (turnAction == null)
            Debug.LogError("HandTurn: there is no action \"TurnHand\" in the Player map. Add it in InputSystem_Actions.");
    }

    void Update()
    {
        if (turnAction == null)
            return;

        if (canTurn && !look.IsLookingAround && !press.IsPressing)
        {
            float input = turnAction.ReadValue<float>();   // Q = -1, E = +1, neither (or both) = 0
            angle += input * turnSpeed * Time.deltaTime;
            angle = Mathf.Clamp(angle, -maxLeft, maxRight);
        }

        // Turning around the up axis (y), like turning a cup on the table.
        // Seen from above, a positive y angle turns clockwise, which is to the right.
        hand.localRotation = Quaternion.Euler(0f, angle, 0f);
    }
}