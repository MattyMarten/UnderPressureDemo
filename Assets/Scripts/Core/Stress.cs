using UnityEngine;
using TMPro;

// Johnny's stress level: 0 to 10 bars.
// Stress is only added (hard choices, pain, things the room does) and only breathing lowers it.
// It never falls by itself.
// For the breathing test it shows the number as plain text on screen.
// Later the stress device on the desk shows it instead (build step 15).
// Put it on its own empty object called "Stress".
public class Stress : MonoBehaviour
{
    public const int MaxBars = 10;

    [Range(0, MaxBars)]
    public int startBars = 7;      // the room's starting value
    public TMP_Text debugText;     // plain text on screen, for testing. Can stay empty.

    // How many bars he has now. Other scripts can read it, but only change it with Add and Lower.
    public int Bars { get; private set; }

    void Start()
    {
        Bars = startBars;
        UpdateText();
    }

    // More stress: a hard choice, pain, the room. Never above 10.
    public void Add(int amount)
    {
        Bars = Mathf.Clamp(Bars + amount, 0, MaxBars);
        UpdateText();
    }

    // Less stress: only breathing calls this. Never below 0.
    public void Lower(int amount)
    {
        Bars = Mathf.Clamp(Bars - amount, 0, MaxBars);
        UpdateText();
    }

    // The name of the level, from the table in the build plan
    public string LevelName
    {
        get
        {
            if (Bars == 0) return "Stable";
            if (Bars <= 4) return "Elevated";
            if (Bars <= 8) return "Critical";
            return "Overload";
        }
    }

    void UpdateText()
    {
        if (debugText != null)
            debugText.text = "Stress: " + Bars + " (" + LevelName + ")";
    }

    // For testing: in Play mode, click the ⋮ on this component in the Inspector and choose one of these
    [ContextMenu("Add 1 bar")]
    void AddOneForTesting() { Add(1); }

    [ContextMenu("Lower 1 bar")]
    void LowerOneForTesting() { Lower(1); }
}
