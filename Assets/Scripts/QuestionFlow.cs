using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Runs one room: question → choice → result → transition → next question.
public class QuestionFlow : MonoBehaviour
{
    public TextAsset questionFile;
    public RoomBase room;
    public VoicePlayer voicePlayer;

    // Clip ids from VoiceLines.csv, played before the first and after the last question
    public List<string> introLines = new List<string>();
    public List<string> endLines = new List<string>();

    void Start()
    {
        StartCoroutine(RunRoom());
    }

        IEnumerator RunRoom()
    {
        List<QuestionData> questions = QuestionLoader.Load(questionFile);

        yield return voicePlayer.PlayAll(introLines);

        foreach (QuestionData question in questions)
        {
            yield return room.AskQuestion(question);
            int choice = room.ChosenOption;

            ChoiceLog.Record(questionFile.name, question, choice);
            Debug.Log(question.id + " → " + ChoiceName(choice));

            yield return room.ShowResult(question, choice);
            yield return room.Transition(question);
        }

        yield return voicePlayer.PlayAll(endLines);

        RoomSummary summary = ChoiceLog.FinishRoom(questionFile.name);
        Debug.Log("Room finished. " + summary.Describe());
    }
    
    static string ChoiceName(int choice)
    {
        if (choice == RoomBase.NoChoice)
            return "none";
        return ((char)('A' + choice)).ToString();
    }
}