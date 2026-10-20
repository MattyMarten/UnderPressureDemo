using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Runs one room: question → choice → result → transition → next question.
public class QuestionFlow : MonoBehaviour
{
    public TextAsset questionFile;
    public RoomBase room;
    public VoicePlayer voicePlayer;
    public BreathFalling breathFalling;   // can stay empty: then his breathing never falls

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
            if (breathFalling != null)
                breathFalling.StartQuestion();

            yield return voicePlayer.PlayAll(question.before);
            room.ShowQuestion(question);   // the papers are there while she reads them
            yield return voicePlayer.PlayAll(question.voice);

            yield return room.AskQuestion(question);
            int choice = room.ChosenOption;

            ChoiceLog.Record(questionFile.name, question, choice);
            Debug.Log(question.id + " → " + ChoiceName(choice));

            yield return room.ShowResult(question, choice);

            // His breathing changes first, so he already breathes that way while she reacts
            string breath = ForAnswer(question.breath, choice);
            if (breath != null)
                yield return voicePlayer.Play("breath:" + breath);

            string reaction = room.PickReaction(question, choice, ForAnswer(question.react, choice));
            if (reaction != null)
                yield return voicePlayer.Play(reaction);

            yield return room.Transition(question);

            if (breathFalling != null)
                yield return breathFalling.EndQuestion();
        }

        yield return voicePlayer.PlayAll(endLines);

        RoomSummary summary = ChoiceLog.FinishRoom(questionFile.name);
        Debug.Log("Room finished. " + summary.Describe());
    }
    
    // The value for this answer from a react or breath list:
    // the one for this exact answer, else the "any" one, else null (nothing)
    static string ForAnswer(Dictionary<string, string> list, int choice)
    {
        string key = ChoiceName(choice);
        if (list.ContainsKey(key))
            return list[key];
        if (list.ContainsKey("any"))
            return list["any"];
        return null;
    }

    static string ChoiceName(int choice)
    {
        if (choice == RoomBase.NoChoice)
            return "none";
        return ((char)('A' + choice)).ToString();
    }
}