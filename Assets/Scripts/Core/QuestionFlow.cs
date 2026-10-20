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
            yield return voicePlayer.PlayAll(question.before);
            yield return voicePlayer.PlayAll(question.voice);

            yield return room.AskQuestion(question);
            int choice = room.ChosenOption;

            ChoiceLog.Record(questionFile.name, question, choice);
            Debug.Log(question.id + " → " + ChoiceName(choice));

            yield return room.ShowResult(question, choice);

            string reaction = room.PickReaction(question, choice, Reaction(question, choice));
            if (reaction != null)
                yield return voicePlayer.Play(reaction);

            yield return room.Transition(question);
        }

        yield return voicePlayer.PlayAll(endLines);

        RoomSummary summary = ChoiceLog.FinishRoom(questionFile.name);
        Debug.Log("Room finished. " + summary.Describe());
    }
    
    // Her reaction to this answer: the one for this exact answer, else the "any" one, else silence (null)
    static string Reaction(QuestionData question, int choice)
    {
        string key = ChoiceName(choice);
        if (question.react.ContainsKey(key))
            return question.react[key];
        if (question.react.ContainsKey("any"))
            return question.react["any"];
        return null;
    }

    static string ChoiceName(int choice)
    {
        if (choice == RoomBase.NoChoice)
            return "none";
        return ((char)('A' + choice)).ToString();
    }
}