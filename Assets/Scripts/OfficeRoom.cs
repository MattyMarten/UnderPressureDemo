using System.Collections;
using UnityEngine;

// The office room. It will put papers on the desk and wait for stamps.
// For now it only has her special reactions.
public class OfficeRoom : RoomBase
{
    // True after she has said "None of them. I see." once in this room
    bool saidNoneOfThem = false;

    public override IEnumerator AskQuestion(QuestionData question)
    {
        // put the paper on the desk
        // wait until every option has a stamp
        // set ChosenOption
        yield break;
    }

    public override string PickReaction(QuestionData question, int choice, string normal)
    {
        // Q12: he accepted "I work well under pressure" on Q7, and now says he freezes (B) or stresses (C)
        if (question.id == "office_12" && (choice == 1 || choice == 2))
        {
            ChoiceRecord q7 = ChoiceLog.Find("office_07");
            if (q7 != null && q7.choice == 0)
                return "OFF_REACT_Q12_CONTRA";
        }

        // The second time she would say "None of them. I see.", she says "None again." instead
        if (normal == "OFF_REACT_DECALL_1")
        {
            if (saidNoneOfThem)
                return "OFF_REACT_DECALL_2";
            saidNoneOfThem = true;
        }

        return normal;
    }
}