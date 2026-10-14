using System.Collections.Generic;
using UnityEngine;

// Temporary test: loads the questions and prints them to the Console.
public class CsvTest : MonoBehaviour
{
    public TextAsset csvFile;

    void Start()
    {
        List<QuestionData> questions = QuestionLoader.Load(csvFile);
        foreach (QuestionData q in questions)
        {
            string line = q.order + ". " + q.id + " (" + q.type + "): " + q.text;
            foreach (QuestionOption o in q.options)
                line += "\n   - " + o.text + " [" + string.Join(", ", o.tags) + "]";
            Debug.Log(line);
        }
    }
}