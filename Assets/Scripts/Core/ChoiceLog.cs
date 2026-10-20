using System.Collections.Generic;

// Remembers every answer in the run, and counts the tags of each room.
public static class ChoiceLog
{
    public static List<ChoiceRecord> records = new List<ChoiceRecord>();
    public static List<RoomSummary> summaries = new List<RoomSummary>();

    // Call this when a new run starts
    public static void Clear()
    {
        records.Clear();
        summaries.Clear();
    }

    // Save one answer
    public static void Record(string room, QuestionData question, int choice)
    {
        var record = new ChoiceRecord();
        record.room = room;
        record.questionId = question.id;
        record.choice = choice;

        if (choice == RoomBase.NoChoice)
            record.tags = question.tagsNone;   // declining everything: the tagsNone column
        else
            record.tags = question.options[choice].tags;

        records.Add(record);
    }

    // His saved answer to one question, or null if he hasn't answered it
    public static ChoiceRecord Find(string questionId)
    {
        foreach (ChoiceRecord record in records)
        {
            if (record.questionId == questionId)
                return record;
        }
        return null;
    }

    // Count the tags of one room and keep the summary
    public static RoomSummary FinishRoom(string room)
    {
        var summary = new RoomSummary();
        summary.room = room;

        foreach (ChoiceRecord record in records)
        {
            if (record.room != room)
                continue;

            foreach (string tag in record.tags)
            {
                if (summary.tagCounts.ContainsKey(tag))
                    summary.tagCounts[tag]++;
                else
                    summary.tagCounts[tag] = 1;
            }
        }

        summaries.Add(summary);
        return summary;
    }
}