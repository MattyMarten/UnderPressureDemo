using TMPro;
using UnityEngine;

// One paper on the desk. It shows one option, or the statement on a single paper.
// It has a "Stamp here:" box. When a stamp mark lands with enough of its face inside the box,
// that stamp counts: it is this paper's stamp, and the box outline goes darker.
// Only the first counting stamp counts. More stamps still leave marks, but change nothing here.
// Green counts on only one paper per question: before green counts, the paper asks its room.
// Put it on the root of the Paper prefab.
public class Paper : MonoBehaviour
{
    public TMP_Text text;   // the Text child

    [Header("Stamp box")]
    public Transform stampBox;                              // the StampBox child: the middle of the box
    public Vector2 boxSize = new Vector2(0.14f, 0.10f);     // width and depth of the box, in meters
    public Renderer[] outline;                              // the four Line cubes
    public Color countedColor = new Color(0.2f, 0.2f, 0.2f);// the outline's colour once a stamp counts
    [Range(0f, 1f)]
    public float neededInside = 0.75f;                      // how much of the face must be inside the box

    // Which option this paper is: 0 = A, 1 = B, 2 = C, 3 = D.
    // A single paper is always 0; its stamp decides accept or decline.
    public int Option { get; private set; }

    // True once a stamp counts on this paper
    public bool HasStamp { get; private set; }

    // The colour of the stamp that counts. Only means something when HasStamp is true.
    public InkColor StampColor { get; private set; }

    // How many points across and along the face we check. 5 × 5 = 25 points.
    const int gridPoints = 5;

    OfficeRoom room;   // the room that laid this paper: it knows the other papers

    public void Setup(int option, string words, OfficeRoom room)
    {
        Option = option;
        text.text = words;
        this.room = room;   // "this.room" is the field above, "room" alone is the parameter
    }

    // StampPress calls this when a mark lands on this paper.
    // position and rotation: where the mark lies (its middle, turned like the stamp).
    public void ReadStamp(Vector3 position, Quaternion rotation, Stamp stamp)
    {
        if (HasStamp)
            return;   // this paper already has its stamp

        float inside = PartInsideBox(position, rotation, stamp.faceSize);

        // Enough inside, but green, and green already counts on another paper: this one doesn't count.
        // (Step 8: she says "You can only accept one paper." here.)
        if (inside >= neededInside && stamp.ink == InkColor.Green && room != null && room.GreenUsed())
        {
            Debug.Log($"Paper {Option}: Green doesn't count, green is already on another paper");
            return;
        }

        if (inside >= neededInside)
        {
            HasStamp = true;
            StampColor = stamp.ink;

            // .material (not .sharedMaterial) gives this renderer its own copy of the material,
            // so only this paper's outline goes darker, not every paper in the game.
            foreach (Renderer line in outline)
                line.material.color = countedColor;

            Debug.Log($"Paper {Option}: {stamp.ink} counts ({inside * 100f:0}% inside the box)");
        }
        else
        {
            Debug.Log($"Paper {Option}: {stamp.ink} doesn't count, only {inside * 100f:0}% inside the box");
        }
    }

    // How much of the stamp face is inside the box: 0 = nothing, 1 = all of it.
    // We put a grid of points on the face and count how many are inside.
    float PartInsideBox(Vector3 position, Quaternion rotation, Vector2 faceSize)
    {
        int insideCount = 0;

        for (int i = 0; i < gridPoints; i++)
        {
            for (int j = 0; j < gridPoints; j++)
            {
                // From -0.5 to 0.5: the left edge of the face, ... the middle, ... the right edge
                float across = (float)i / (gridPoints - 1) - 0.5f;
                float along = (float)j / (gridPoints - 1) - 0.5f;

                // The point on the face, in the face's own space (x = width, z = depth, flat: y = 0)
                Vector3 onFace = new Vector3(across * faceSize.x, 0f, along * faceSize.y);

                // Into the world: turn it like the mark, then move it to the mark
                Vector3 world = position + rotation * onFace;

                // Into the box's own space: (0, 0, 0) is the middle of the box
                Vector3 inBox = stampBox.InverseTransformPoint(world);

                if (Mathf.Abs(inBox.x) <= boxSize.x / 2f && Mathf.Abs(inBox.z) <= boxSize.y / 2f)
                    insideCount++;
            }
        }

        return (float)insideCount / (gridPoints * gridPoints);
    }

    // Draws the box in yellow in the Scene view (not in the game), so you can check it matches the lines
    void OnDrawGizmosSelected()
    {
        if (stampBox == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = stampBox.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxSize.x, 0f, boxSize.y));
    }
}