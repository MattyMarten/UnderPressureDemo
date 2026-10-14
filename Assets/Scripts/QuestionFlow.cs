using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Runs one room: question → choice → result → transition → next question.
public class QuestionFlow : MonoBehaviour
{
    public TextAsset questionFile;
    public RoomBase room;

    void Start()
    {
        StartCoroutine(RunRoom());
    }

    IEnumerator RunRoom()
    {
        List<QuestionData> questions = QuestionLoader.Load(questionFile);

        foreach (QuestionData question in questions)
        {
            yield return room.AskQuestion(question);
            int choice = room.ChosenOption;

            // The choice log will be called here later
            Debug.Log(question.id + " → " + ChoiceName(choice));

            yield return room.ShowResult(question, choice);
            yield return room.Transition(question);
        }

        Debug.Log("Room finished.");
    }

    static string ChoiceName(int choice)
    {
        if (choice == RoomBase.NoChoice)
            return "none";
        return ((char)('A' + choice)).ToString();
    }
}