using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Johnny's own sound channel: his one-time sounds (mumbles, crying, laughing...),
// and his breathing (through JohnnyBreathing).
// His clips start and return at once, so her voice can play over them.
// His caption shows above her subtitle while his clip plays.
//
// A one-time sound can be named two ways:
//   JOH_CRY_02  plays exactly that clip
//   JOH_CRY     plays a random JOH_CRY_.. clip, never the same one twice in a row
public class JohnnyVoice : MonoBehaviour
{
    public AudioSource audioSource;
    public TMP_Text captionText;                 // his caption, above her subtitle
    public JohnnyBreathing breathing;            // his breathing, on its own channel
    public string clipFolder = "Voice/Johnny";   // inside a Resources folder
    public float breathCaptionTime = 3f;         // seconds a breathing caption stays

    // Every clip by its exact name: "JOH_CRY_02" → clip
    Dictionary<string, AudioClip> clipsByName = new Dictionary<string, AudioClip>();
    // Every group by its name: "JOH_CRY" → JOH_CRY_01, JOH_CRY_02...
    Dictionary<string, ClipVariants> groups = new Dictionary<string, ClipVariants>();

    Coroutine clearRoutine;                      // the timer that clears his caption

    void Awake()
    {
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>(clipFolder))
        {
            clipsByName[clip.name] = clip;

            string group = ClipVariants.GroupName(clip.name);
            if (!groups.ContainsKey(group))
                groups[group] = new ClipVariants();
            groups[group].Add(clip);
        }

        if (clipsByName.Count == 0)
            Debug.LogWarning("JohnnyVoice: no clips in Resources/" + clipFolder + ". Is the name right?");

        captionText.text = "";
    }

    // Starts one of his sounds and does not wait for it.
    // If one of his sounds is still playing, it is cut off.
    // His breathing pauses while it plays, because you can hear him breathe in it already.
    // An empty caption means no caption.
    public void Play(string id, string caption)
    {
        AudioClip clip = FindClip(id);
        if (clip == null)
        {
            Debug.LogWarning("JohnnyVoice: no audio clip called \"" + id + "\" (or " + id + "_01...) in Resources/" + clipFolder + ".");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();

        breathing.PauseFor(clip.length);
        ShowCaption(caption, clip.length);
    }

    // The exact clip if one has this name, else a random one from the group with this name
    AudioClip FindClip(string id)
    {
        if (clipsByName.ContainsKey(id))
            return clipsByName[id];
        if (groups.ContainsKey(id))
            return groups[id].Pick();
        return null;
    }

    // Changes his breathing state. firstClip is a clip to start with, or "" for random.
    // The caption shows only if the state really changes.
    public void SetBreathing(int level, string firstClip, string caption)
    {
        bool changed = breathing.SetLevel(level, firstClip);
        if (changed && caption != "")
            ShowCaption(caption, breathCaptionTime);
    }

    // Shows his caption in square brackets for some seconds, then clears it.
    // An empty caption clears it at once.
    void ShowCaption(string caption, float seconds)
    {
        // Stop the timer of the old caption, so it can't clear the new one
        if (clearRoutine != null)
            StopCoroutine(clearRoutine);

        if (caption == "")
            captionText.text = "";
        else
            captionText.text = "[" + caption + "]";

        clearRoutine = StartCoroutine(ClearCaptionAfter(seconds));
    }

    // Waits, then clears his caption
    IEnumerator ClearCaptionAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        captionText.text = "";
        clearRoutine = null;
    }
}