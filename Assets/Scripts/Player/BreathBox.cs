using UnityEngine;

// The breathing box: a horizontal bar that grows from the middle out to both edges
// while he breathes in, and shrinks back to the middle while he breathes out.
// It is hidden while he isn't breathing, and fades in when he starts.
// It only shows what Breathing measures; it doesn't judge anything itself.
// Put it on the BreathBox object, together with a Canvas Group.
// Its child BreathFill is the coloured part that grows.
public class BreathBox : MonoBehaviour
{
    public Breathing breathing;
    public RectTransform fill;     // BreathFill
    public float fadeTime = 0.2f;  // seconds to fade fully in or out

    CanvasGroup group;             // fades the box and everything under it at once

    void Start()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null)
            Debug.LogError("BreathBox: add a Canvas Group component to the BreathBox object.");
        else
            group.alpha = 0f;      // hidden when the game starts
    }

    void Update()
    {
        // Anchors are measured as parts of the parent box: 0 = its left edge, 0.5 = its middle, 1 = its right edge.
        // Half of the fill goes to each side of the middle.
        // Fill 0: both sides at 0.5, so nothing shows. Fill 1: 0 to 1, the whole box.
        float half = breathing.Fill / 2f;
        fill.anchorMin = new Vector2(0.5f - half, 0f);
        fill.anchorMax = new Vector2(0.5f + half, 1f);

        if (group == null)
            return;

        // Visible while he breathes (in or out), hidden while Breathing waits for a new breath
        bool breathingNow = breathing.CurrentPhase != Breathing.Phase.Waiting;
        float targetAlpha = breathingNow ? 1f : 0f;

        // MoveTowards: a step of the same size each frame, so the fade takes fadeTime seconds
        group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, Time.deltaTime / fadeTime);
    }
}