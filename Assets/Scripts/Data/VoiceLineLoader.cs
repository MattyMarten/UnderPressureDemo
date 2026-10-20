using System.Collections.Generic;
using UnityEngine;

// Turns VoiceLines.csv into a lookup: clip id → voice line.
// Columns are found by their name in the header, so new language columns
// can be added without changing this code.
// Writes a warning to the Console for every mistake it finds.
public static class VoiceLineLoader
{
    public static Dictionary<string, VoiceLine> Load(TextAsset file)
    {
        var lines = new Dictionary<string, VoiceLine>();
        List<string[]> rows = CsvReader.Read(file.text);

        if (rows.Count == 0)
        {
            Debug.LogWarning(file.name + ": the file is empty.");
            return lines;
        }

        // Row 0 is the header: id, speaker, en, ...
        string[] header = rows[0];
        int idColumn = FindColumn(header, "id");
        int speakerColumn = FindColumn(header, "speaker");

        if (idColumn < 0 || speakerColumn < 0)
        {
            Debug.LogWarning(file.name + ": the header needs an \"id\" and a \"speaker\" column.");
            return lines;
        }

        for (int r = 1; r < rows.Count; r++)
        {
            string[] row = rows[r];
            string where = file.name + ", line " + (r + 1);

            // Skip empty lines
            if (row.Length == 1 && row[0].Trim() == "")
                continue;

            if (row.Length != header.Length)
            {
                Debug.LogWarning(where + ": " + row.Length + " cells, expected " + header.Length + ". Missing quotes around a comma?");
                continue;
            }

            var line = new VoiceLine();
            line.id = row[idColumn].Trim();
            line.speaker = row[speakerColumn].Trim();

            if (line.id == "")
            {
                Debug.LogWarning(where + ": no id.");
                continue;
            }

            // Every other column is a language
            for (int c = 0; c < header.Length; c++)
            {
                if (c == idColumn || c == speakerColumn)
                    continue;
                line.subtitles[header[c].Trim()] = row[c].Trim();
            }

            if (lines.ContainsKey(line.id))
            {
                Debug.LogWarning(where + ": the id \"" + line.id + "\" is used twice.");
                continue;
            }

            lines.Add(line.id, line);
        }

        return lines;
    }

    // Returns the position of the column with this name, or -1 if there is none
    static int FindColumn(string[] header, string name)
    {
        for (int c = 0; c < header.Length; c++)
        {
            if (header[c].Trim().ToLower() == name)
                return c;
        }
        return -1;
    }
}
