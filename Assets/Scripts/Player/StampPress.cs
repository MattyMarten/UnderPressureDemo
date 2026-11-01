using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Presses the stamp in his hand down with Space.
// Space pushes it down onto whatever is under it (a paper or the desk) and leaves an ink mark there.
// It stays down while Space is held and comes back up when Space is let go.
// While it is pressing, the hand can't move or turn, and the stamp can't be let go.
// It doesn't decide answers: the paper will read its stamp in the next step.
// Put it on HandArea, next to DeskHand.
public class StampPress : MonoBehaviour
{
    public SeatedLook look;            // no stamping while he looks around (Shift)
    public StampHolder holder;         // to know which stamp is in his hand
    public InkMark inkMarkPrefab;      // the InkMark prefab from Assets/Prefabs
    public Transform deskMarks;        // marks that don't land on a paper become children of this (OfficeSet)
    public float pressTime = 0.08f;    // seconds to go down, and again to come up
    public float maxReach = 0.3f;      // how far below the stamp it looks for something to stamp, in meters

    // True from the moment the press starts until the stamp is back up.
    // DeskHand, HandTurn and StampHolder read it.
    public bool IsPressing { get; private set; }

    // How far down the wrist is pushed now, in meters. DeskHand reads it.
    public float Depth { get; private set; }

    InputAction stampAction;
    int marksMade;   // how many marks so far in this room, so each new one goes on top

    void Start()
    {
        stampAction = InputSystem.actions.FindAction("Player/Stamp");
        if (stampAction == null)
            Debug.LogError("StampPress: there is no action \"Stamp\" in the Player map. Add it in InputSystem_Actions, bound to Space.");
    }

    void Update()
    {
        if (stampAction == null)
            return;

        // Space was just pressed, he isn't already pressing, and he isn't looking around
        if (stampAction.WasPressedThisFrame() && !IsPressing && !look.IsLookingAround)
        {
            Stamp stamp = holder.Held;

            // Only a stamp that has arrived in his hand. A gliding one isn't attached yet.
            if (stamp != null && !stamp.IsGliding)
                StartCoroutine(Press(stamp));
        }
    }

    IEnumerator Press(Stamp stamp)
    {
        IsPressing = true;

        // What is under the stamp, and how far down it is
        RaycastHit hit;
        bool found = FindSurface(stamp, out hit);
        float drop = found ? hit.distance - 0.01f : 0f;   // the ray starts 1 cm above the stamp's face
        drop = Mathf.Max(drop, 0f);

        // Down
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressTime;
            Depth = Mathf.Lerp(0f, drop, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        Depth = drop;

        // The face touches: ink, if there is any left
        if (found)
            LeaveMark(stamp, hit);

        // Stay down while Space is held
        while (stampAction.IsPressed())
            yield return null;

        // Up
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressTime;
            Depth = Mathf.Lerp(drop, 0f, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        Depth = 0f;

        IsPressing = false;
    }

    // Shoots a ray straight down from the stamp's face and returns the first thing it hits
    // that isn't the stamp itself or his arm
    bool FindSurface(Stamp stamp, out RaycastHit surface)
    {
        Vector3 start = stamp.transform.position + Vector3.up * 0.01f;
        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, maxReach + 0.01f);

        // RaycastAll gives the hits in no particular order, so sort them: nearest first
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Transform thing = hit.collider.transform;
            if (thing.IsChildOf(stamp.transform) || thing.IsChildOf(transform))
                continue;   // its own Base, or a part of his arm: look further down

            surface = hit;
            return true;
        }

        surface = default;   // nothing found
        return false;
    }

    // Puts an ink mark where the face touched, turned like the stamp
    void LeaveMark(Stamp stamp, RaycastHit hit)
    {
        float strength;
        if (!stamp.UseInk(out strength))
            return;   // the stamp is dry: no mark

        // The mark lies flat on the surface (its up = the surface's up),
        // and its forward points the same way as the stamp's forward
        Vector3 forward = Vector3.ProjectOnPlane(stamp.transform.forward, hit.normal);
        Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);

        // A mark on a paper becomes part of that paper, so it leaves with it.
        // Anything else (the desk) keeps it under deskMarks for the whole room.
        Transform parent = deskMarks;
        Paper paper = hit.collider.GetComponentInParent<Paper>();
        if (paper != null)
            parent = paper.transform;

        // 1 mm above the surface, so it doesn't flicker with it
        Vector3 position = hit.point + hit.normal * 0.001f;

        InkMark mark = Instantiate(inkMarkPrefab, position, rotation, parent);
        mark.Setup(stamp.inkMaterial, strength, marksMade);
        marksMade++;
    }
}