using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Temporary test room: shows questions in the Console and reads answers from the keyboard.
// Keys 1-4 pick an option, 0 declines everything.
// It is built on OfficeRoom, so it uses the office's special reactions.
public class KeyboardTestRoom : OfficeRoom
{
    public override IEnumerator AskQuestion(QuestionData question)
    {
        string text = question.id + " (" + question.type + "): " + question.text;
        for (int i = 0; i < question.options.Count; i++)
            text += "\n   " + (i + 1) + " = " + question.options[i].text;
        if (question.type != QuestionType.Single)
            text += "\n   0 = decline everything";
        Debug.Log(text);

        while (true)
        {
            yield return null;   // wait one frame, then check the keys again

            int key = PressedNumber();
            if (key >= 1 && key <= question.options.Count)
            {
                ChosenOption = key - 1;
                yield break;
            }
            if (key == 0 && question.type != QuestionType.Single)
            {
                ChosenOption = NoChoice;
                yield break;
            }
        }
    }

    public override IEnumerator Transition(QuestionData question)
    {
        Debug.Log("Stamped!");
        Debug.Log("(the paper goes away)");
        yield return new WaitForSeconds(0.5f);
    }

    // Returns the number key pressed this frame, or -1 if none
    static int PressedNumber()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return -1;
        if (keyboard.digit0Key.wasPressedThisFrame) return 0;
        if (keyboard.digit1Key.wasPressedThisFrame) return 1;
        if (keyboard.digit2Key.wasPressedThisFrame) return 2;
        if (keyboard.digit3Key.wasPressedThisFrame) return 3;
        if (keyboard.digit4Key.wasPressedThisFrame) return 4;
        return -1;
    }
}