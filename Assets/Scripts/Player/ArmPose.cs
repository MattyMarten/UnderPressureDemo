using UnityEngine;

// Builds his right arm each frame from three points: Shoulder (fixed), Elbow and Wrist.
// The wrist is moved by DeskHand. The elbow slides forward and back with the wrist,
// so it helps the hand reach further. Each arm piece is a cube stretched between two points.
// The hand is not part of this: it is a child of the Wrist and keeps pointing forward.
// Put it on HandArea. Shoulder, Elbow, Wrist and both cubes are children of HandArea.
public class ArmPose : MonoBehaviour
{
    public Transform shoulder;   // stays where you put it
    public Transform elbow;      // moved here. Where you put it = the elbow when the wrist is in the middle.
    public Transform wrist;      // moved by DeskHand

    public Transform upperArm;   // cube from shoulder to elbow
    public Transform forearm;    // cube from elbow to wrist

    [Range(0f, 1f)]
    public float forwardFollow = 0.7f;   // how much of the wrist's forward/back movement the elbow copies
    [Range(0f, 1f)]
    public float sideFollow = 0f;        // how much of its left/right movement. 0 = none.
    public float thickness = 0.06f;      // how thick the arm cubes are, in meters

    Vector3 elbowStart;   // the elbow's position as placed in the editor

    void Start()
    {
        elbowStart = elbow.localPosition;
    }

    // LateUpdate runs after every Update in the scene.
    // So DeskHand has already moved the wrist this frame, and the arm never lags one frame behind.
    void LateUpdate()
    {
        // The wrist's distance from the middle of the area (HandArea's own space)
        Vector3 w = wrist.localPosition;

        // The elbow copies part of it. Its height never changes.
        elbow.localPosition = new Vector3(
            elbowStart.x + w.x * sideFollow,
            elbowStart.y,
            elbowStart.z + w.z * forwardFollow);

        // First the elbow, then the cubes, in this order
        Stretch(upperArm, shoulder.position, elbow.position);
        Stretch(forearm, elbow.position, wrist.position);
    }

    // Puts a cube between two points: in the middle, pointing from a to b, as long as the distance
    void Stretch(Transform piece, Vector3 a, Vector3 b)
    {
        float length = Vector3.Distance(a, b);
        if (length < 0.001f)
            return;   // the points are on top of each other: no direction to point in

        piece.position = (a + b) / 2f;
        piece.rotation = Quaternion.LookRotation(b - a);   // the cube's forward (z) now points from a to b
        piece.localScale = new Vector3(thickness, thickness, length);
    }
}
