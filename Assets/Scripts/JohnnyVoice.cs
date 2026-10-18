using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Johnny's own sound channel: his mumbles, and later his breathing.
// His clips start and return at once, so her voice can play over them.
// His caption shows above her subtitle while his clip plays.
public class JohnnyVoice : MonoBehaviour
{
    public AudioSource audioSource;
    public TMP_Text captionText;                 // his caption, above her subtitle
    public string clipFolder = "Voice/Johnny";   // inside a Resources folder

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

        // Stop the timer of the clip we just cut off, so it can't clear the new caption
        if (clearRoutine != null)
            StopCoroutine(clearRoutine);

        if (caption == "")
            captionText.text = "";
        else
            captionText.text = "[" + caption + "]";

        clearRoutine = StartCoroutine(ClearCaptionAfter(clip.length));
    }

    // Waits until his clip has ended, then clears his caption
    IEnumerator ClearCaptionAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        captionText.text = "";
        clearRoutine = null;
    }
}