using System.Collections.Generic;

// One saved answer: "office, office_08, B, family"
[System.Serializable]
public class ChoiceRecord
{
    public string room;
    public string questionId;
    public int choice;        // 0 = A, 1 = B, 2 = C, 3 = D, or RoomBase.NoChoice
    public string[] tags;     // the tags of the chosen option
}

// The counted tags of one finished room
public class RoomSummary
{
    public string room;
    public Dictionary<string, int> tagCounts = new Dictionary<string, int>();

    public string Describe()
    {
        string text = room + ":";
        foreach (KeyValuePair<string, int> pair in tagCounts)
            text += " " + pair.Key + " " + pair.Value + ",";
        return text;
    }
}