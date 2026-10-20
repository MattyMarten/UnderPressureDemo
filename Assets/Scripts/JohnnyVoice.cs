using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Johnny's own sound channel: his mumbles, and his breathing (through JohnnyBreathing).
// His clips start and return at once, so her voice can play over them.
// His caption shows above her subtitle while his clip plays.
public class JohnnyVoice : MonoBehaviour
{
    public AudioSource audioSource;
    public TMP_Text captionText;                 // his caption, above her subtitle
    public JohnnyBreathing breathing;            // his breathing, on its own channel
    public string clipFolder = "Voice/Johnny";   // inside a Resources folder
    public float breathCaptionTime = 3f;         // seconds a breathing caption stays

    Dictionary<string, AudioClip> clipsByName = new Dictionary<string, AudioClip>();
    Coroutine clearRoutine;                      // the timer that clears his caption

    void Awake()
    {
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>(clipFolder))
            clipsByName[clip.name] = clip;

        if (clipsByName.Count == 0)
            Debug.LogWarning("JohnnyVoice: no clips in Resources/" + clipFolder + ". Is the name right?");

        captionText.text = "";
    }

    // Starts one of his clips and does not wait for it.
    // If one of his clips is still playing, it is cut off.
    // His breathing pauses while the clip plays, because you can hear him breathe in the mumble already.
    // An empty caption means no caption.
    public void Play(string id, string caption)
    {
        if (!clipsByName.ContainsKey(id))
        {
            Debug.LogWarning("JohnnyVoice: no audio clip called \"" + id + "\" in Resources/" + clipFolder + ".");
            return;
        }

        AudioClip clip = clipsByName[id];
        audioSource.clip = clip;
        audioSource.Play();

        breathing.PauseFor(clip.length);
        ShowCaption(caption, clip.length);
    }

    // Changes his breathing level. The caption shows only if the level really changes.
    public void SetBreathing(int level, string caption)
    {
        bool changed = breathing.SetLevel(level);
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