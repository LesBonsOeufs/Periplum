using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FadeAnim_TMP : FadeAnim
{
    private TextMeshPro tmp;

    protected override float Alpha 
    { 
        get => tmp.alpha;
        set => tmp.alpha = value;
    }

    protected override void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        base.Awake();
    }
}
