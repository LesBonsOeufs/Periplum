using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeAnim_Sprite : FadeAnim
{
    private SpriteRenderer spriteRenderer;

    protected override float Alpha 
    { 
        get => spriteRenderer.color.a;

        set
        {
            Color lColor = spriteRenderer.color;
            lColor.a = value;
            spriteRenderer.color = lColor;
        }
    }

    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        base.Awake();
    }
}
