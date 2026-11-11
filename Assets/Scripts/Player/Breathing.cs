using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Johnny's breathing, controlled by the player.
// Hold Shift (Breathe) to breathe in, about 4 seconds. Release to breathe out, about 6 seconds.
// Each half is judged on its own: Perfect, Good or Bad.
// A cycle (in + out) gets the worse of its two halves.
// After "cycles per check" cycles, the worst cycle lowers the stress:
// Perfect = 2 bars, Good = 1 bar, Bad = no change.
// A Bad in cancels the breath: the cycle is already Bad, so there is no out.
// The out ends when Shift is pressed again. If nobody presses in time, the out counts as Bad.
// The result word (Perfect, Good, Bad) shows in its colour when Shift is released and pressed again.
// Off by more than "silent margin" shows nothing, so quick taps don't flash text.
// The word disappears after "result show time" seconds.
// Put it on the Stress object, next to Stress.
public class Breathing : MonoBehaviour
{
    // Perfect is the best, Bad the worst. The order matters: a bigger number is worse.
    public enum Result { Perfect, Good, Bad }

    // What he is doing now. The box (step 3c) reads it.
    public enum Phase { Waiting, In, Out }

    public Stress stress;
    public TMP_Text resultText;          // shows the last result in its colour. Can stay empty.

    [Header("Timing (seconds)")]
    public float inSeconds = 4f;
    public float outSeconds = 6f;
    public float perfectMargin = 0.2f;   // off by this much or less = Perfect
    public float goodMargin = 0.5f;      // off by this much or less = Good. More = Bad.
    public float silentMargin = 1f;      // off by more than this: still Bad, but no text

    [Header("Lowering stress")]
    public int cyclesPerCheck = 1;       // how many cycles before stress is lowered
    public int perfectLowers = 2;        // bars
    public int goodLowers = 1;           // bars

    [Header("Colours")]
    public Color perfectColor = Color.green;
    public Color goodColor = new Color(0.3f, 0.6f, 1f);   // a light blue, easier to read than pure blue
    public Color badColor = Color.red;
    public float resultShowTime = 0.8f;  // seconds the result word stays on screen

    public bool canBreathe = true;       // later: off when the room doesn't allow it

    public Phase CurrentPhase { get; private set; } = Phase.Waiting;

    // How full the box is, 0 to 1: fills during the in, empties during the out.
    public float Fill
    {
        get
        {
            if (CurrentPhase == Phase.In) return Mathf.Clamp01(phaseTime / inSeconds);
            if (CurrentPhase == Phase.Out) return outStartFill * Mathf.Clamp01(1f - phaseTime / outSeconds);
            return 0f;
        }
    }

    InputAction breatheAction;
    float phaseTime;          // seconds since this in or out started
    Result inResult;          // the judged in of the cycle that is going on now
    float outStartFill;       // how full the box was when the in ended: the out shrinks from there
    float resultTimer;        // seconds left before the result word disappears
    int cyclesDone;           // cycles since the last check
    Result worstInCheck;      // the worst cycle since the last check

    void Start()
    {
        // "Breathe" in the Player map of InputSystem_Actions: Shift
        breatheAction = InputSystem.actions.FindAction("Player/Breathe");
        if (breatheAction == null)
            Debug.LogError("Breathing: there is no action \"Breathe\" in the Player map. Add it in InputSystem_Actions, bound to Shift.");

        if (resultText != null)
            resultText.text = "";
    }

    void Update()
    {
        HideResultWhenTimeIsUp();

        if (breatheAction == null || !canBreathe)
            return;

        bool held = breatheAction.IsPressed();
        phaseTime += Time.deltaTime;

        switch (CurrentPhase)
        {
            case Phase.Waiting:
                // Not breathing yet. Pressing Shift starts an in.
                if (held)
                    StartPhase(Phase.In);
                break;

            case Phase.In:
                // Shift released: the in is over. Judge it.
                if (!held)
                {
                    inResult = Judge(phaseTime, inSeconds);
                    Show(inResult, phaseTime, inSeconds);

                    if (inResult == Result.Bad)
                    {
                        // The cycle is Bad whatever the out does, so stop here.
                        // Waiting hides the box, and a new press starts a new breath.
                        FinishCycle(Result.Bad);
                        StartPhase(Phase.Waiting);
                    }
                    else
                    {
                        // Perfect or Good: breathe out, starting from where the box is now
                        outStartFill = Fill;
                        StartPhase(Phase.Out);
                    }
                }
                break;

            case Phase.Out:
                if (held)
                {
                    // Shift pressed again: the out is over, and a new in starts right away
                    Result outResult = Judge(phaseTime, outSeconds);
                    Show(outResult, phaseTime, outSeconds);
                    FinishCycle(outResult);
                    StartPhase(Phase.In);
                }
                else if (phaseTime > outSeconds + silentMargin)
                {
                    // He stopped breathing: count it as Bad, quietly, and wait for a new breath
                    Show(Result.Bad, phaseTime, outSeconds);
                    FinishCycle(Result.Bad);
                    StartPhase(Phase.Waiting);
                }
                break;
        }
    }

    void StartPhase(Phase phase)
    {
        CurrentPhase = phase;
        phaseTime = 0f;
    }

    // How far the real time was from the target time decides the result
    Result Judge(float actual, float target)
    {
        float off = Mathf.Abs(actual - target);   // Abs: too short and too long count the same
        if (off <= perfectMargin) return Result.Perfect;
        if (off <= goodMargin) return Result.Good;
        return Result.Bad;
    }

    // A cycle is done (in + out). Its result is the worse of the two halves.
    void FinishCycle(Result outResult)
    {
        Result cycle = Worse(inResult, outResult);

        // The first cycle of a check sets the worst; later cycles can only make it worse
        worstInCheck = cyclesDone == 0 ? cycle : Worse(worstInCheck, cycle);
        cyclesDone++;

        if (cyclesDone >= cyclesPerCheck)
        {
            if (worstInCheck == Result.Perfect) stress.Lower(perfectLowers);
            else if (worstInCheck == Result.Good) stress.Lower(goodLowers);
            // Bad: no change

            cyclesDone = 0;
        }
    }

    // The enum's order: Perfect = 0, Good = 1, Bad = 2. The bigger number is the worse result.
    static Result Worse(Result a, Result b)
    {
        return a > b ? a : b;
    }

    // Counts down while a word is on screen, and clears it at 0
    void HideResultWhenTimeIsUp()
    {
        if (resultTimer <= 0f)
            return;   // no word on screen

        resultTimer -= Time.deltaTime;
        if (resultTimer <= 0f && resultText != null)
            resultText.text = "";
    }

    // Shows the result word in its colour, or nothing if he was far off
    void Show(Result result, float actual, float target)
    {
        if (resultText == null)
            return;

        if (Mathf.Abs(actual - target) > silentMargin)
        {
            resultText.text = "";   // empty: an old word shouldn't stay on screen
            return;
        }

        resultText.text = result.ToString();   // the enum's name as text: "Perfect", "Good" or "Bad"
        if (result == Result.Perfect) resultText.color = perfectColor;
        else if (result == Result.Good) resultText.color = goodColor;
        else resultText.color = badColor;

        resultTimer = resultShowTime;   // start counting down; a new word starts it again
    }
}