using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeAnim_Sprite : FadeAnim
{
    private SpriteRenderer spriteRenderer;

    public override float Alpha 
    { 
        get => spriteRenderer.color.a;

        protected set
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
