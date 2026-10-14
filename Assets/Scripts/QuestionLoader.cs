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
    const int Voice = 12;
    const int ColumnCount = 13;

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
        q.voice = row[Voice].Trim();

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

        if (q.voice == "")
            Debug.LogWarning(where + ": no voice clip yet.");

        return q;
    }

    // Makes one option and cuts its tags at the | sign
    static QuestionOption MakeOption(string text, string tagCell)
    {
        var option = new QuestionOption();
        option.text = text;

        var tags = new List<string>();
        foreach (string tag in tagCell.Split('|'))
        {
            if (tag.Trim() != "")
                tags.Add(tag.Trim());
        }
        option.tags = tags.ToArray();
        return option;
    }
}