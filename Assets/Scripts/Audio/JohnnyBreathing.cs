using System.Collections.Generic;
using UnityEngine;

// Johnny's breathing, on its own sound channel, so it keeps going while she talks.
// The state picks which clips play: state 1 plays JOH_BREATH_1_01, JOH_BREATH_1_02...
// It plays one clip after another, picked at random, never the same one twice in a row.
// A state change fades the old breathing out first.
//
// The states (the number is the first number in the file name):
//   -1  silent
//    0  idle
//    1  bit stressed   (like idle, a bit louder and a bit faster)
//    2  fast breathing
//    3  stressed       (with a little mumble)
//    4  scared         (uneven breathing)
//    5  pain           (crying and mumbling)
//    6  really painful (mumbled curses and crying)
public class JohnnyBreathing : MonoBehaviour
{
    public const int Silent = -1;

    public AudioSource audioSource;
    public string clipFolder = "Voice/Johnny";   // inside a Resources folder
    public float fadeTime = 0.3f;                // seconds to fade out, and to fade in

    // The state that is playing now, and the state the script asked for
    public int Level { get; private set; } = Silent;
    int wantedLevel = Silent;

    // True when the breathing must fade out and start again:
    // a new state, or the same state starting with a chosen clip
    bool switchWaiting;
    string firstClipName = "";   // the clip to start with after the switch, "" = random

    // state → its clips: 1 → JOH_BREATH_1_01, JOH_BREATH_1_02, JOH_BREATH_1_03
    Dictionary<int, ClipVariants> clipsByLevel = new Dictionary<int, ClipVariants>();

    float fullVolume;     // the volume set on the AudioSource in the Inspector
    float pausedUntil;    // Time.time when the breathing may start again

    const string Prefix = "JOH_BREATH_";

    void Awake()
    {
        foreach (AudioClip clip in Resources.LoadAll<AudioClip>(clipFolder))
        {
            if (!clip.name.StartsWith(Prefix))
                continue;   // a mumble or another sound, not a breath

            // "JOH_BREATH_1_03" → "1_03" → "1"
            string rest = clip.name.Substring(Prefix.Length);
            string number = rest.Split('_')[0];
            int level;
            if (!int.TryParse(number, out level))
            {
                Debug.LogWarning("JohnnyBreathing: \"" + clip.name + "\" has no state number. Name it like JOH_BREATH_1_01.");
                continue;
            }

            if (!clipsByLevel.ContainsKey(level))
                clipsByLevel[level] = new ClipVariants();
            clipsByLevel[level].Add(clip);
        }

        audioSource.loop = false;     // we start each clip ourselves
        fullVolume = audioSource.volume;
        audioSource.volume = 0f;
    }

    // Asks for a state. firstClip is a clip to start with, like "JOH_BREATH_5_03",
    // or "" to pick at random. Returns true only if the state really changes,
    // so the caller knows when to show a caption.
    public bool SetLevel(int level, string firstClip)
    {
        bool sameLevel = (level == wantedLevel);
        if (sameLevel && firstClip == "")
            return false;   // already in this state, nothing to do

        if (level != Silent && !clipsByLevel.ContainsKey(level))
        {
            Debug.LogWarning("JohnnyBreathing: no clips for state " + level + ". Add one named " + Prefix + level + "_01.");
            return false;
        }

        wantedLevel = level;
        firstClipName = firstClip;
        switchWaiting = true;
        return !sameLevel;
    }

    // Stops the breathing for a while, for example while he mumbles.
    // Afterwards a new clip of the same state starts and fades in.
    public void PauseFor(float seconds)
    {
        pausedUntil = Time.time + seconds;
        audioSource.Stop();
        audioSource.volume = 0f;
    }

    void Update()
    {
        // 1. A switch is waiting: fade the old breathing out, then start the new state
        if (switchWaiting)
        {
            if (audioSource.isPlaying && audioSource.volume > 0f)
            {
                FadeTowards(0f);
                return;
            }
            audioSource.Stop();
            audioSource.volume = 0f;
            Level = wantedLevel;
            switchWaiting = false;
        }

        // 2. Paused while he mumbles
        if (Time.time < pausedUntil)
            return;

        // 3. Nothing is playing: start the chosen first clip, or else a random one
        if (Level != Silent && !audioSource.isPlaying)
        {
            if (firstClipName != "")
            {
                PlayClip(FindFirstClip());
                firstClipName = "";
            }
            else
            {
                PlayClip(clipsByLevel[Level].Pick());
            }
        }

        // 4. Fade in after a switch or a pause. At full volume this changes nothing.
        FadeTowards(fullVolume);
    }

    // The chosen first clip, or a random one if it doesn't exist in this state
    AudioClip FindFirstClip()
    {
        AudioClip clip = clipsByLevel[Level].PickNamed(firstClipName);
        if (clip != null)
            return clip;

        Debug.LogWarning("JohnnyBreathing: state " + Level + " has no clip called \"" + firstClipName + "\". Playing a random one.");
        return clipsByLevel[Level].Pick();
    }

    void PlayClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    // Moves the volume a small step towards the target, so it takes fadeTime seconds in all
    void FadeTowards(float target)
    {
        float step = fullVolume / fadeTime * Time.deltaTime;
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, target, step);
    }
}