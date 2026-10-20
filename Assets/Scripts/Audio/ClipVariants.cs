using System.Collections.Generic;
using UnityEngine;

// A group of recordings of the same sound: JOH_CRY_01, JOH_CRY_02, JOH_CRY_03...
// Pick() gives a random one, but never the same one twice in a row.
// Only for Johnny's sounds. Her lines are always one fixed clip.
public class ClipVariants
{
    List<AudioClip> clips = new List<AudioClip>();
    AudioClip last;    // the one that played last time

    public void Add(AudioClip clip)
    {
        clips.Add(clip);
    }

    // A random clip, but not the one that played last time
    public AudioClip Pick()
    {
        if (clips.Count == 1)
        {
            last = clips[0];
            return last;
        }

        AudioClip clip;
        do
        {
            clip = clips[Random.Range(0, clips.Count)];
        }
        while (clip == last);

        last = clip;
        return clip;
    }

    // One exact clip by its name, or null if this group doesn't have it.
    // It also counts as "last", so the next Pick() won't repeat it.
    public AudioClip PickNamed(string name)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip.name == name)
            {
                last = clip;
                return clip;
            }
        }
        return null;
    }

    // The name of the group a clip belongs to: the clip name without its number at the end.
    // "JOH_CRY_02" → "JOH_CRY", "JOH_BREATH_1_03" → "JOH_BREATH_1".
    // A name with no number at the end is its own group: "JOH_MUM_WAKE" → "JOH_MUM_WAKE".
    public static string GroupName(string clipName)
    {
        int lastUnderscore = clipName.LastIndexOf('_');
        if (lastUnderscore < 0)
            return clipName;

        string ending = clipName.Substring(lastUnderscore + 1);
        int number;
        if (int.TryParse(ending, out number))
            return clipName.Substring(0, lastUnderscore);

        return clipName;
    }
}
