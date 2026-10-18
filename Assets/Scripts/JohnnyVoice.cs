using System.Collections.Generic;
using UnityEngine;

// Johnny's own sound channel: his mumbles, and later his breathing.
// His clips start and return at once, so her voice can play over them.
public class JohnnyVoice : MonoBehaviour
{
    public AudioSource audioSource;
    public string clipFolder = "Voice/Johnny";   // inside a Resources folder

    Dictionary<string, AudioClip> clipsByName = new Dictionary<string, AudioClip>();

    void Awake()
    {
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>(clipFolder))
            clipsByName[clip.name] = clip;

        if (clipsByName.Count == 0)
            Debug.LogWarning("JohnnyVoice: no clips in Resources/" + clipFolder + ". Is the name right?");
    }

    // Starts one of his clips and does not wait for it.
    // If one of his clips is still playing, it is cut off.
    public void Play(string id)
    {
        if (!clipsByName.ContainsKey(id))
        {
            Debug.LogWarning("JohnnyVoice: no audio clip called \"" + id + "\" in Resources/" + clipFolder + ".");
            return;
        }

        audioSource.clip = clipsByName[id];
        audioSource.Play();
    }
}
