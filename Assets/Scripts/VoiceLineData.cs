using System.Collections.Generic;

// One voice clip, matching one row of VoiceLines.csv.
public class VoiceLine
{
    public string id;         // same as the audio file name, e.g. "OFF_Q04"
    public string speaker;    // "Her", "Johnny", "Host"

    // The subtitle in each language: "en" → "Noted."
    public Dictionary<string, string> subtitles = new Dictionary<string, string>();
}
