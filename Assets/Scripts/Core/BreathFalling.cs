using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// One falling rule: "in state 2, after 1 quiet question, go to state 1"
[System.Serializable]
public class FallRule
{
    public int from;
    public int to;
    public int afterQuestions = 2;
}

// Lets Johnny's breathing calm down when nothing stresses him for a while.
// A quiet question is one where nothing set his breathing (no breath: anywhere).
// A state with no rule never falls. Put one on each room's QuestionFlow object.
public class BreathFalling : MonoBehaviour
{
    public JohnnyBreathing breathing;
    public VoicePlayer voicePlayer;
    public List<FallRule> rules = new List<FallRule>();

    int quietQuestions = 0;    // quiet questions in a row
    int setCountAtStart;       // breathing.SetCount when this question started

    // QuestionFlow calls this before each question
    public void StartQuestion()
    {
        setCountAtStart = breathing.SetCount;
    }

    // QuestionFlow calls this after each question.
    // Counts the question as quiet or not, then follows a rule if one fits.
    public IEnumerator EndQuestion()
    {
        if (breathing.SetCount == setCountAtStart)
            quietQuestions++;
        else
            quietQuestions = 0;   // something happened: start counting again

        FallRule rule = FindRule(breathing.Level);
        if (rule != null && quietQuestions >= rule.afterQuestions)
        {
            Debug.Log("Breathing falls from " + rule.from + " to " + rule.to + " after " + quietQuestions + " quiet questions.");
            yield return voicePlayer.Play("breath:" + rule.to);   // same path as every breath: entry, caption included
            quietQuestions = 0;   // the new state counts its own quiet questions
        }
    }

    // The rule for this state, or null if it has none
    FallRule FindRule(int level)
    {
        foreach (FallRule rule in rules)
        {
            if (rule.from == level)
                return rule;
        }
        return null;
    }
}
