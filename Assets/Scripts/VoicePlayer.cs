using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Plays one voice clip and shows its subtitle while it plays.
// Rooms wait for it with: yield return voicePlayer.Play("OFF_Q04");
public class VoicePlayer : MonoBehaviour
{
    public TextAsset voiceLinesFile;     // VoiceLines.csv
    public AudioSource audioSource;
    public TMP_Text subtitleText;
    public string language = "en";       // which subtitle column to show
    public float gapBetweenLines = 0.3f; // silence between clips in PlayAll, in seconds

    // Folders inside any Resources folder. Every clip in them is loaded.
    public List<string> clipFolders = new List<string> { "Voice/Office", "Voice/Johnny" };

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

    // Plays several clips one after another, with a short gap between them
    public IEnumerator PlayAll(IEnumerable<string> ids)
    {
        foreach (string id in ids)
        {
            yield return Play(id);
            yield return new WaitForSeconds(gapBetweenLines);
        }
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