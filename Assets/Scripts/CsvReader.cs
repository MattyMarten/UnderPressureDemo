using System.Collections.Generic;
using System.Text;

// Turns the text of a CSV file into rows of cells.
public static class CsvReader
{
    public static List<string[]> Read(string fileText)
    {
        fileText = fileText.TrimStart('\uFEFF');   // remove hidden mark some programs add
        char separator = FindSeparator(fileText);

        var rows = new List<string[]>();
        var cells = new List<string>();
        var cell = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < fileText.Length; i++)
        {
            char c = fileText[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // Two quotes inside quotes mean one real quote character
                    if (i + 1 < fileText.Length && fileText[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    cell.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == separator)
            {
                cells.Add(cell.ToString());
                cell.Clear();
            }
            else if (c == '\n')
            {
                cells.Add(cell.ToString());
                cell.Clear();
                rows.Add(cells.ToArray());
                cells.Clear();
            }
            else if (c != '\r')
            {
                cell.Append(c);
            }
        }

        // The last row may not end with a new line
        if (cell.Length > 0 || cells.Count > 0)
        {
            cells.Add(cell.ToString());
            rows.Add(cells.ToArray());
        }

        return rows;
    }

    // Looks at the first line: if it has more ; than , the file uses ;
    static char FindSeparator(string fileText)
    {
        int lineEnd = fileText.IndexOf('\n');
        string firstLine = lineEnd >= 0 ? fileText.Substring(0, lineEnd) : fileText;

        int commas = 0;
        int semicolons = 0;
        foreach (char c in firstLine)
        {
            if (c == ',') commas++;
            if (c == ';') semicolons++;
        }
        return semicolons > commas ? ';' : ',';
    }
}