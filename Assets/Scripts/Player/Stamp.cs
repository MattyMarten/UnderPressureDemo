using UnityEngine;

// The two ink colours. Green = accept, red = decline.
// An enum is a small list of named choices. In the Inspector it shows as a drop-down.
public enum InkColor { Green, Red }

// One stamp on the desk: its ink.
// It stands in its own ink pool: every press uses some ink, and standing on its spot refills it.
// Picking up and moving is done by Grabbable (on the same object), pressing by StampPress.
// Put it on GreenStamp and on RedStamp, next to Grabbable.
public class Stamp : MonoBehaviour
{
    public InkColor ink = InkColor.Green;                // which colour it stamps. The paper reads this
    public Material inkMaterial;                         // what its marks look like (Ink_Green or Ink_Red)
    public int inkUses = 5;                              // how many marks it can make before it needs its pool again
    public Vector2 faceSize = new Vector2(0.07f, 0.05f); // the face: width (left-right) and depth, in meters. Same as Base

    int inkLeft;              // how many marks it can still make
    Grabbable grabbable;      // on the same object: tells us when the stamp is back on its spot

    void Awake()
    {
        inkLeft = inkUses;   // it starts full, standing in its pool
        grabbable = GetComponent<Grabbable>();
    }

    void Update()
    {
        // Standing on its spot = standing in its pool: full again.
        // (Grabbable sets IsHome to true when it has glided all the way back.)
        if (grabbable != null && grabbable.IsHome)
            inkLeft = inkUses;
    }

    // Uses ink for one mark. Gives back false if it is dry (no mark).
    // strength: 1 for the first mark, then fainter: 5 uses → 1, 0.8, 0.6, 0.4, 0.2
    public bool UseInk(out float strength)
    {
        if (inkLeft <= 0)
        {
            strength = 0f;
            return false;
        }

        strength = (float)inkLeft / inkUses;   // (float) so 4 / 5 is 0.8, not 0
        inkLeft--;
        return true;
    }
}