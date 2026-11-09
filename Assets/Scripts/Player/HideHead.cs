using UnityEngine;

// Hides Johnny's head, so the camera (at his eyes) doesn't see the inside of his face.
// It shrinks the head bone to almost nothing. Everything on that bone (face, hair, eyes) shrinks with it.
// Side effect: his shadow has no head either.
// Put it on Johnny (the model's root object).
public class HideHead : MonoBehaviour
{
    public Transform head;       // the head bone: mixamorig:Head
    public bool hidden = true;   // untick to see the head again (for example to check the model)

    Vector3 normalScale;         // the head's size before we change it

    void Start()
    {
        normalScale = head.localScale;
    }

    // LateUpdate: after the Animator and the IK have moved his bones this frame
    void LateUpdate()
    {
        // 0.001, not 0: a scale of exactly 0 can give Unity maths errors
        head.localScale = hidden ? normalScale * 0.001f : normalScale;
    }
}