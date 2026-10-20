using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

// Plays one voice clip and shows its subtitle while it plays.
// Rooms wait for it with: yield return voicePlayer.Play("OFF_Q04");
// Johnny's lines go to JohnnyVoice and don't wait. "wait:1.5" is a pause of 1.5 seconds.
// "breath:1" sets Johnny's breathing to state 1 and doesn't wait.
// "breath:5_03" goes to state 5 and starts with JOH_BREATH_5_03. "breath:-1" is silence.
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

        // His breathing state: "breath:1", "breath:5_03" or "breath:-1".
        // Its caption is the line JOH_BREATH_1 in VoiceLines.csv. Silent has no caption.
        if (id.StartsWith("breath:"))
        {
            int level;
            string firstClip;
            if (ReadBreath(id, out level, out firstClip))
            {
                string caption = "";
                if (level != JohnnyBreathing.Silent)
                    caption = Subtitle("JOH_BREATH_" + level);
                johnnyVoice.SetBreathing(level, firstClip, caption);
            }
            yield break;
        }

        // His sounds play on his own channel, and we don't wait for them
        if (IsJohnny(id))
        {
            johnnyVoice.Play(id, Subtitle(LineId(id)));
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

    // "breath:1"    → state 1, firstClip ""
    // "breath:5_03" → state 5, firstClip "JOH_BREATH_5_03"
    // "breath:-1"   → silent
    // Returns false (and warns) if it can't read it.
    bool ReadBreath(string id, out int level, out string firstClip)
    {
        string value = id.Substring("breath:".Length).Trim();   // "5_03"
        string[] parts = value.Split('_');                       // ["5", "03"]
        firstClip = "";

        bool ok = int.TryParse(parts[0], out level)
               && level >= JohnnyBreathing.Silent
               && parts.Length <= 2;

        if (!ok)
        {
            Debug.LogWarning("VoicePlayer: \"" + id + "\" is not a breathing state. Write it like breath:1 or breath:5_03");
            return false;
        }

        if (parts.Length == 2)
            firstClip = "JOH_BREATH_" + value;
        return true;
    }

    // The row in VoiceLines.csv for this clip: its own row if it has one,
    // else the row of its group. "JOH_CRY_02" has no row, so it uses "JOH_CRY".
    string LineId(string id)
    {
        if (lines.ContainsKey(id))
            return id;
        return ClipVariants.GroupName(id);
    }

    // True if VoiceLines.csv says this sound is Johnny's
    bool IsJohnny(string id)
    {
        string row = LineId(id);
        return lines.ContainsKey(row) && lines[row].speaker == "Johnny";
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