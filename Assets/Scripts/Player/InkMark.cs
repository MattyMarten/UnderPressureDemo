using UnityEngine;

// One ink mark left by a stamp. It shows the stamp's ink material (a colour, or later an image)
// and is fainter when the stamp had less ink left.
// Newer marks are always drawn on top of older ones (see "order").
// Put it on the root of the InkMark prefab.
public class InkMark : MonoBehaviour
{
    public Renderer image;   // the Image child (a Quad)

    // ink: the stamp's ink material. strength: 1 = full ink, smaller = fainter.
    // order: 0 for the first mark in the room, then 1, 2, 3...
    public void Setup(Material ink, float strength, int order)
    {
        // A copy of the material just for this mark,
        // so making this mark fainter doesn't change the stamp's material or other marks
        Material copy = new Material(ink);

        Color color = copy.color;
        color.a *= strength;   // a = alpha: 1 = solid, 0 = invisible
        copy.color = color;

        // When it is drawn. See-through things are normally sorted by distance to the camera,
        // which can put an older mark on top. A higher Render Queue is always drawn later, so on top.
        // 3000 is where see-through things start; Unity allows up to 5000.
        copy.renderQueue = 3000 + Mathf.Min(order, 1999);

        image.material = copy;
    }
}