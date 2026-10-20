using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The office room. It puts papers on the desk and will wait for stamps.
// It also has her special reactions.
public class OfficeRoom : RoomBase
{
    public Paper paperPrefab;            // the Paper prefab from Assets/Prefabs
    public Transform paperCenter;        // the middle of the row of papers, on the desk
    public float paperSpacing = 0.24f;   // meters from the middle of one paper to the next

    // The papers on the desk now
    protected List<Paper> papers = new List<Paper>();

    // True after she has said "None of them. I see." once in this room
    bool saidNoneOfThem = false;

    // Puts one paper per option on the desk, in a row centered on paperCenter.
    // A single question gets one paper with the statement on it.
    public override void ShowQuestion(QuestionData question)
    {
        bool single = (question.type == QuestionType.Single);
        int count = single ? 1 : question.options.Count;

        for (int i = 0; i < count; i++)
        {
            // How far from the middle: 2 papers → -0.5, 0.5; 3 papers → -1, 0, 1 (times the spacing)
            float offset = (i - (count - 1) / 2f) * paperSpacing;
            Vector3 position = paperCenter.position + paperCenter.right * offset;

            Paper paper = Instantiate(paperPrefab, position, paperCenter.rotation, paperCenter);

            string words = single ? question.text : question.options[i].text;
            paper.Setup(i, words);
            papers.Add(paper);
        }
    }

    public override IEnumerator AskQuestion(QuestionData question)
    {
        // Stamping comes in a later step. Until then nobody can answer here,
        // so we wait for ever instead of giving a wrong answer.
        Debug.LogWarning("OfficeRoom: stamping isn't built yet. Use KeyboardTestRoom to answer.");
        while (true)
            yield return null;
    }

    // Takes the papers away. Later they will move away instead of disappearing.
    public override IEnumerator Transition(QuestionData question)
    {
        foreach (Paper paper in papers)
            Destroy(paper.gameObject);
        papers.Clear();
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