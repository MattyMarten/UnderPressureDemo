using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

// Plays one voice clip and shows its subtitle while it plays.
// Rooms wait for it with: yield return voicePlayer.Play("OFF_Q04");
// Johnny's lines go to JohnnyVoice and don't wait. "wait:1.5" is a pause of 1.5 seconds.
// "breath:1" sets Johnny's breathing to level 1 and doesn't wait.
public class VoicePlayer : MonoBehaviour
{
    public TextAsset voiceLinesFile;     // VoiceLines.csv
    public AudioSource audioSource;
    public TMP_Text subtitleText;
    public JohnnyVoice johnnyVoice;      // his own channel
    public string language = "en";       // which subtitle column to show
    public float gapBetweenLines = 0.3f; // silence between clips in PlayAll, in seconds

    // Folders inside any Resources folder. Every clip in them is loaded.
    // Johnny's folder is not here: JohnnyVoice loads his clips.
    public List<string> clipFolders = new List<string> { "Voice/Office" };

    public string testLine = "OFF_Q04";  // for the right-click test below

    Dictionary<string, VoiceLine> lines;
    Dictionary<string, AudioClip> clipsByName = new Dictionary<string, AudioClip>();

    void Awake()
    {
        lines = VoiceLineLoader.Load(voiceLinesFile);

        // Load every clip in each folder and make a lookup: file name → clip
        foreach (string folder in clipFolders)
        {
            AudioClip[] found = Resources.LoadAll<AudioClip>(folder);

            if (found.Length == 0)
                Debug.LogWarning("VoicePlayer: no clips in Resources/" + folder + ". Is the name right?");
            else
                Debug.Log("VoicePlayer: " + found.Length + " clips from Resources/" + folder);

            foreach (AudioClip clip in found)
            {
                if (clipsByName.ContainsKey(clip.name))
                    Debug.LogWarning("VoicePlayer: the clip \"" + clip.name + "\" exists twice. Each clip should be in one folder only.");
                else
                    clipsByName.Add(clip.name, clip);
            }
        }

        subtitleText.text = "";
    }

    // Plays one clip, shows its subtitle, and waits until the clip has ended
    public IEnumerator Play(string id)
    {
        // A pause: "wait:1.5"
        if (id.StartsWith("wait:"))
        {
            yield return new WaitForSeconds(WaitSeconds(id));
            yield break;
        }

        // His breathing level: "breath:1". Its caption is the line JOH_BREATH_1 in VoiceLines.csv
        if (id.StartsWith("breath:"))
        {
            int level;
            if (BreathLevel(id, out level))
                johnnyVoice.SetBreathing(level, Subtitle("JOH_BREATH_" + level));
            yield break;
        }

        // His lines play on his own channel, and we don't wait for them
        if (IsJohnny(id))
        {
            johnnyVoice.Play(id, Subtitle(id));
            yield break;
        }

        if (!clipsByName.ContainsKey(id))
        {
            Debug.LogWarning("VoicePlayer: no audio clip called \"" + id + "\". Is it in one of the clip folders?");
            yield break;
        }

        AudioClip clip = clipsByName[id];
        subtitleText.text = Subtitle(id);
        audioSource.clip = clip;
        audioSource.Play();

        yield return new WaitForSeconds(clip.length);

        subtitleText.text = "";
    }

    // Plays several clips one after another, with a short gap after each of her lines
    public IEnumerator PlayAll(IEnumerable<string> ids)
    {
        foreach (string id in ids)
        {
            yield return Play(id);

            // No gap after a pause, a breathing change or one of his sounds, so they add no extra time
            if (!id.StartsWith("wait:") && !id.StartsWith("breath:") && !IsJohnny(id))
                yield return new WaitForSeconds(gapBetweenLines);
        }
    }

    // "breath:1" → 1. Returns false (and warns) if there is no number after "breath:"
    bool BreathLevel(string id, out int level)
    {
        string number = id.Substring("breath:".Length).Trim();
        if (int.TryParse(number, out level) && level >= 0)
            return true;

        Debug.LogWarning("VoicePlayer: \"" + id + "\" is not a breathing level. Write it like breath:1");
        return false;
    }

    // True if VoiceLines.csv says this line is Johnny's
    bool IsJohnny(string id)
    {
        return lines.ContainsKey(id) && lines[id].speaker == "Johnny";
    }

    // "wait:1.5" → 1.5. Always reads a dot as the decimal sign, whatever the computer's language.
    float WaitSeconds(string id)
    {
        string number = id.Substring("wait:".Length).Trim();
        float seconds;
        if (float.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds))
            return seconds;

        Debug.LogWarning("VoicePlayer: \"" + id + "\" is not a pause. Write it like wait:1.5");
        return 0f;
    }

    // The subtitle in the chosen language, or in English if that one is empty
    string Subtitle(string id)
    {
        if (!lines.ContainsKey(id))
        {
            Debug.LogWarning("VoicePlayer: \"" + id + "\" is not in " + voiceLinesFile.name + ".");
            return "";
        }

        VoiceLine line = lines[id];
        if (line.subtitles.ContainsKey(language) && line.subtitles[language] != "")
            return line.subtitles[language];
        if (line.subtitles.ContainsKey("en"))
            return line.subtitles["en"];
        return "";
    }

    // In Play mode: click the ⋮ menu on this component and choose "Play test line"
    [ContextMenu("Play test line")]
    void PlayTestLine()
    {
        StartCoroutine(Play(testLine));
    }
}