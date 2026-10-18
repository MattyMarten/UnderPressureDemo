using System.Collections.Generic;

// The three kinds of paper.
public enum QuestionType
{
    Single,  // one statement: accept or decline
    Pair,    // two options
    Pick     // three or four options, only one can be accepted
}

// One option on a paper, with the tags it gives when chosen.
[System.Serializable]
public class QuestionOption
{
    public string text;
    public string[] tags;
}

// One question, matching one row of a room's CSV file.
[System.Serializable]
public class QuestionData
{
    public string id;           // fixed name, e.g. "office_08"
    public int order;           // when it appears in the room
    public QuestionType type;
    public string text;         // the statement or question
    public List<QuestionOption> options = new List<QuestionOption>();

    // Voice clip ids from VoiceLines.csv
    public string[] before;     // played before the question
    public string[] voice;      // the question itself, can be several clips

    // Her reaction after the answer: "A", "B", "C", "D", "none" or "any" → clip id
    public Dictionary<string, string> react = new Dictionary<string, string>();
}