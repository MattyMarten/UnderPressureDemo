using System.Collections;
using UnityEngine;

// The office room. Empty for now: it will put papers on the desk and wait for stamps.
public class OfficeRoom : RoomBase
{
    public override IEnumerator AskQuestion(QuestionData question)
    {
        // put the paper on the desk
        // wait until every option has a stamp
        // set ChosenOption
        yield break;
    }
}