using TMPro;
using UnityEngine;

// One paper on the desk. It shows one option, or the statement on a single paper.
// Later it also remembers which stamp it got.
// Put it on the root of the Paper prefab.
public class Paper : MonoBehaviour
{
    public TMP_Text text;   // the Text child

    // Which option this paper is: 0 = A, 1 = B, 2 = C, 3 = D.
    // A single paper is always 0; its stamp decides accept or decline.
    public int Option { get; private set; }

    public void Setup(int option, string words)
    {
        Option = option;
        text.text = words;
    }
}
