using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_FitKeyboard : MonoBehaviour
{
    private readonly string[] bannedKeys = new string[] { "ESC", "<", ">", "^" };

    private TextMeshProUGUI tmp;
    private string key;

#if !UNITY_SWITCH || UNITY_EDITOR
    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        key = tmp.text;
    }

    private void OnEnable()
    {
        foreach (string lKey in bannedKeys)
        {
            if (key == lKey)
                return;
        }

        try
        {
            //Might be incorrect in some situations (example: M)
            tmp.text = Keyboard.current.FindKeyOnCurrentKeyboardLayout(key).name.ToUpperInvariant();
        }
        catch { }
    }
#endif
}