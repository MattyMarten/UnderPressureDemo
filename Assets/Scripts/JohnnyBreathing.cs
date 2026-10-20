using System.Collections.Generic;
using UnityEngine;

// Johnny's breathing, on its own sound channel, so it keeps going while she talks.
// The level picks which clips play: level 0 plays JOH_BREATH_0_a, JOH_BREATH_0_b...
// It plays one clip after another. With several clips for a level it picks one at random,
// never the same one twice in a row. A level change fades the old breathing out first.
public class JohnnyBreathing : MonoBehaviour
{
    public const int Silent = -1;          // no breathing at all

    public AudioSource audioSource;
    public string clipFolder = "Voice/Johnny";   // inside a Resources folder
    public float fadeTime = 0.3f;          // seconds to fade out, and to fade in

    // The level that is playing now, and the level the script asked for
    public int Level { get; private set; } = Silent;
    int wantedLevel = Silent;

    // level → its clips: 0 → [JOH_BREATH_0_a, JOH_BREATH_0_b]
    Dictionary<int, List<AudioClip>> clipsByLevel = new Dictionary<int, List<AudioClip>>();

    AudioClip lastClip;       // so the same clip doesn't play twice in a row
    float fullVolume;         // the volume set on the AudioSource in the Inspector
    float pausedUntil;        // Time.time when the breathing may start again

    const string Prefix = "JOH_BREATH_";

    void Awake()
    {
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>(clipFolder))
        {
            if (!clip.name.StartsWith(Prefix))
                continue;   // a mumble, not a breath

            // "JOH_BREATH_1_a" → "1_a" → "1"
            string rest = clip.name.Substring(Prefix.Length);
            string number = rest.Split('_')[0];
            int level;
            if (!int.TryParse(number, out level))
            {
                Debug.LogWarning("JohnnyBreathing: \"" + clip.name + "\" has no level number. Name it like JOH_BREATH_1_a.");
                continue;
            }

            if (!clipsByLevel.ContainsKey(level))
                clipsByLevel[level] = new List<AudioClip>();
            clipsByLevel[level].Add(clip);
        }

        audioSource.loop = false;     // we start each clip ourselves
        fullVolume = audioSource.volume;
        audioSource.volume = 0f;
    }

    // Asks for a new level. Returns true only if the level really changes,
    // so the caller knows when to show a caption.
    public bool SetLevel(int level)
    {
        if (level == wantedLevel)
            return false;

        if (level != Silent && !clipsByLevel.ContainsKey(level))
        {
            Debug.LogWarning("JohnnyBreathing: no clips for level " + level + ". Add one named " + Prefix + level + "_a.");
            return false;
        }

        wantedLevel = level;
        return true;
    }

    // Stops the breathing for a while, for example while he mumbles.
    // Afterwards a new clip of the same level starts and fades in.
    public void PauseFor(float seconds)
    {
        pausedUntil = Time.time + seconds;
        audioSource.Stop();
        audioSource.volume = 0f;
    }

    void Update()
    {
        // 1. A new level is waiting: fade the old one out, then switch
        if (wantedLevel != Level)
        {
            if (audioSource.isPlaying && audioSource.volume > 0f)
            {
                FadeTowards(0f);
                return;
            }
            audioSource.Stop();
            audioSource.volume = 0f;
            Level = wantedLevel;
            lastClip = null;
        }

        // 2. Paused while he mumbles
        if (Time.time < pausedUntil)
            return;

        // 3. The last clip has ended: start the next one
        if (Level != Silent && !audioSource.isPlaying)
            PlayNext();

        // 4. Fade in after a switch or a pause. At full volume this changes nothing.
        FadeTowards(fullVolume);
    }

    void PlayNext()
    {
        audioSource.clip = PickClip(clipsByLevel[Level]);
        audioSource.Play();
        lastClip = audioSource.clip;
    }

    // A random clip from the list, but not the one that just played
    AudioClip PickClip(List<AudioClip> clips)
    {
        if (clips.Count == 1)
            return clips[0];

        AudioClip clip;
        do
        {
            clip = clips[Random.Range(0, clips.Count)];
        }
        while (clip == lastClip);
        return clip;
    }

    // Moves the volume a small step towards the target, so it takes fadeTime seconds in all
    void FadeTowards(float target)
    {
        float step = fullVolume / fadeTime * Time.deltaTime;
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, target, step);
    }
}
