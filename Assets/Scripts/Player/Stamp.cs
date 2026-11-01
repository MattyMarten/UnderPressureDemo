using System.Collections;
using UnityEngine;

// One stamp on the desk. It remembers its own spot, glides into the hand when picked up,
// and glides back to its spot when let go.
// Each stamp moves itself, so one can glide home while the other is picked up.
// It also stands in its own ink pool: every press uses some ink, and going home refills it.
// Put it on GreenStamp and on RedStamp.
public class Stamp : MonoBehaviour
{
    public float glideTime = 0.15f;   // seconds to glide into the hand, or back home
    public Material inkMaterial;      // what its marks look like (Ink_Green or Ink_Red)
    public int inkUses = 5;           // how many marks it can make before it needs its pool again

    // Its own spot, measured inside its parent (OfficeSet)
    Transform homeParent;
    Vector3 homePosition;
    Quaternion homeRotation;

    Coroutine gliding;   // the glide running now, or null
    int inkLeft;         // how many marks it can still make

    // True while it glides into the hand or back home. StampPress only presses a stamp that has arrived.
    public bool IsGliding => gliding != null;

    void Awake()
    {
        homeParent = transform.parent;
        homePosition = transform.localPosition;
        homeRotation = transform.localRotation;
        inkLeft = inkUses;   // it starts full, standing in its pool
    }

    public void GoToHand(Transform grip)
    {
        StartGlide(GlideToHand(grip));
    }

    public void GoHome()
    {
        StartGlide(GlideHome());
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

    // Stops a glide that is still running, so two glides never fight over the stamp
    void StartGlide(IEnumerator glide)
    {
        if (gliding != null)
            StopCoroutine(gliding);

        // Back under OfficeSet while it glides, staying exactly where it is now.
        // Otherwise, coming out of the hand, it would still move with the hand.
        transform.SetParent(homeParent, true);

        gliding = StartCoroutine(glide);
    }

    IEnumerator GlideToHand(Transform grip)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float t = 0f;   // 0 = where it started, 1 = in the hand
        while (t < 1f)
        {
            t += Time.deltaTime / glideTime;
            float eased = Mathf.SmoothStep(0f, 1f, t);   // starts slowly, ends slowly

            // grip.position is read every frame, so it catches the hand even while the hand moves
            transform.position = Vector3.Lerp(startPosition, grip.position, eased);
            transform.rotation = Quaternion.Slerp(startRotation, grip.rotation, eased);
            yield return null;
        }

        // Arrived: from now on it is part of the hand and moves and turns with it
        transform.SetParent(grip);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gliding = null;
    }

    IEnumerator GlideHome()
    {
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        float t = 0f;   // 0 = where it was let go, 1 = home
        while (t < 1f)
        {
            t += Time.deltaTime / glideTime;
            float eased = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition = Vector3.Lerp(startPosition, homePosition, eased);
            transform.localRotation = Quaternion.Slerp(startRotation, homeRotation, eased);
            yield return null;
        }

        transform.localPosition = homePosition;
        transform.localRotation = homeRotation;
        inkLeft = inkUses;   // back in its pool: full again
        gliding = null;
    }
}