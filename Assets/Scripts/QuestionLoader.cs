using System;
using System.Collections.Generic;
using UnityEngine;

// Turns a room's CSV file into a list of questions.
// Writes a warning to the Console for every mistake it finds.
public static class QuestionLoader
{
    // Where each column sits in a row (counting from 0)
    const int Id = 0;
    const int Order = 1;
    const int Type = 2;
    const int Text = 3;
    const int OptionA = 4;   // options A-D are columns 4, 5, 6, 7
    const int TagsA = 8;     // tags A-D are columns 8, 9, 10, 11
    const int Before = 12;
    const int Voice = 13;
    const int React = 14;
    const int ColumnCount = 15;

    public static List<QuestionData> Load(TextAsset file)
    {
        var questions = new List<QuestionData>();
        var usedIds = new HashSet<string>();
        List<string[]> rows = CsvReader.Read(file.text);

        // Start at 1, because row 0 is the header
        for (int r = 1; r < rows.Count; r++)
        {
            string[] row = rows[r];
            string where = file.name + ", line " + (r + 1);

            // Skip empty lines
            if (row.Length == 1 && row[0].Trim() == "")
                continue;

            if (row.Length != ColumnCount)
            {
                Debug.LogWarning(where + ": " + row.Length + " cells, expected " + ColumnCount + ". Missing quotes around a comma?");
                continue;
            }

            QuestionData q = ReadRow(row, where);
            if (q == null)
                continue;

            if (!usedIds.Add(q.id))
            {
                Debug.LogWarning(where + ": the id \"" + q.id + "\" is used twice.");
                continue;
            }

            questions.Add(q);
        }

        // Put the questions in playing order
        questions.Sort((a, b) => a.order.CompareTo(b.order));
        return questions;
    }

    // Turns one row into a question. Returns null if the row has a mistake.
    static QuestionData ReadRow(string[] row, string where)
    {
        var q = new QuestionData();
        q.id = row[Id].Trim();
        q.text = row[Text].Trim();
        q.before = SplitList(row[Before]);
        q.voice = SplitList(row[Voice]);

        if (!int.TryParse(row[Order].Trim(), out q.order))
        {
            Debug.LogWarning(where + ": order \"" + row[Order] + "\" is not a number.");
            return null;
        }

        if (!Enum.TryParse(row[Type].Trim(), true, out q.type))
        {
            Debug.LogWarning(where + ": unknown type \"" + row[Type] + "\". Use single, pair or pick.");
            return null;
        }

        if (q.type == QuestionType.Single)
        {
            // A = accept the statement, B = decline it
            q.options.Add(MakeOption("accept", row[TagsA]));
            q.options.Add(MakeOption("decline", row[TagsA + 1]));
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                string optionText = row[OptionA + i].Trim();
                if (optionText != "")
                    q.options.Add(MakeOption(optionText, row[TagsA + i]));
            }

            int count = q.options.Count;
            bool wrongCount = (q.type == QuestionType.Pair && count != 2)
                           || (q.type == QuestionType.Pick && count < 3);
            if (wrongCount)
            {
                Debug.LogWarning(where + ": a " + q.type + " question has " + count + " options. That doesn't fit.");
                return null;
            }
        }

        if (q.voice.Length == 0)
            Debug.LogWarning(where + ": no voice clip yet.");

        ReadReactions(q, row[React], where);
        return q;
    }

    // Reads a cell like "A:OFF_REACT_08|B:OFF_REACT_02|none:OFF_REACT_DECALL_1"
    static void ReadReactions(QuestionData q, string cell, string where)
    {
        foreach (string entry in SplitList(cell))
        {
            int colon = entry.IndexOf(':');
            if (colon < 0)
            {
                Debug.LogWarning(where + ": the reaction \"" + entry + "\" has no \":\". Write it like A:OFF_REACT_02.");
                continue;
            }

            string key = entry.Substring(0, colon).Trim();
            string clip = entry.Substring(colon + 1).Trim();

            if (!IsReactionKey(key, q.options.Count))
            {
                Debug.LogWarning(where + ": unknown reaction key \"" + key + "\". Use A to D (only as many as the options), none or any.");
                continue;
            }
            if (q.react.ContainsKey(key))
            {
                Debug.LogWarning(where + ": the reaction key \"" + key + "\" is used twice.");
                continue;
            }
            q.react.Add(key, clip);
        }
    }

    // A, B, C, D (only as many as the question has options), none or any
    static bool IsReactionKey(string key, int optionCount)
    {
        if (key == "none" || key == "any")
            return true;
        if (key.Length == 1 && key[0] >= 'A' && key[0] < 'A' + optionCount)
            return true;
        return false;
    }

    // Makes one option and cuts its tags at the | sign
    static QuestionOption MakeOption(string text, string tagCell)
    {
        var option = new QuestionOption();
        option.text = text;

        option.tags = SplitList(tagCell);
        return option;
    }

    // Cuts a cell at the | sign: "a | b||c" → ["a", "b", "c"]. An empty cell gives an empty list.
    static string[] SplitList(string cell)
    {
        var parts = new List<string>();
        foreach (string part in cell.Split('|'))
        {
            if (part.Trim() != "")
                parts.Add(part.Trim());
        }
        return parts.ToArray();
    }
}