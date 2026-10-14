using System.Collections;
using UnityEngine;

// The base for every room. The question flow asks the room to do each part
// of the rhythm, and each room does the parts in its own way.
public abstract class RoomBase : MonoBehaviour
{
    public const int NoChoice = -1;   // he declined every option ("none")

    // Set by AskQuestion: 0 = A, 1 = B, 2 = C, 3 = D, or NoChoice
    public int ChosenOption { get; protected set; }

    // Show the question and wait until he has answered. Every room must have this.
    public abstract IEnumerator AskQuestion(QuestionData question);

    // Show what his choice caused. A room that needs this replaces it.
    public virtual IEnumerator ShowResult(QuestionData question, int choice)
    {
        yield break;
    }

    // Take the old question away before the next one. A room that needs this replaces it.
    public virtual IEnumerator Transition(QuestionData question)
    {
        yield break;
    }
}