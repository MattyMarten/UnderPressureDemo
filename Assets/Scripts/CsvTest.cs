using UnityEngine;

// Temporary test: prints every row of a CSV file to the Console.
public class CsvTest : MonoBehaviour
{
    public TextAsset csvFile;

    void Start()
    {
        var rows = CsvReader.Read(csvFile.text);
        foreach (string[] row in rows)
        {
            Debug.Log(row.Length + " cells: " + string.Join(" | ", row));
        }
    }
}