using UnityEngine;

// An invisible box on the desk that a held object can move in.
// The object follows his gaze, but never leaves this box.
// Several objects can share one zone (both stamps use the same one).
// Put it on an empty object lying on the desk surface. Its position is the middle of the box,
// at desk height. Keep its Scale at 1, so "size" is in real meters.
public class GrabZone : MonoBehaviour
{
    public Vector2 size = new Vector2(0.6f, 0.35f);   // width (left-right) and depth (near-far), in meters

    // The desk surface under the zone, as an endless flat plane. Grabbable shoots his gaze at it.
    public Plane Floor => new Plane(transform.up, transform.position);

    // The closest point inside the zone, on the desk surface
    public Vector3 Clamp(Vector3 worldPoint)
    {
        // From the world into the zone's own space: (0, 0, 0) is the zone's middle
        Vector3 local = transform.InverseTransformPoint(worldPoint);

        local.x = Mathf.Clamp(local.x, -size.x / 2f, size.x / 2f);
        local.z = Mathf.Clamp(local.z, -size.y / 2f, size.y / 2f);
        local.y = 0f;   // on the desk surface

        // And back into the world
        return transform.TransformPoint(local);
    }

    // Draws the zone as a cyan box in the Scene view (not in the game), so you can place and size it
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 0f, size.y));
    }
}
