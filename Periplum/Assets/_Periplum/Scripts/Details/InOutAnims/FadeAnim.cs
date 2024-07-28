using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeAnim : MonoBehaviour, IInOutAnim
{
    [SerializeField] private float duration = 0.5f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Color lColor = spriteRenderer.color;
        lColor.a = 0f;
        spriteRenderer.color = lColor;
    }

    public void In()
    {
        spriteRenderer.DOFade(1f, duration);
    }

    public void Out()
    {
        spriteRenderer.DOFade(0f, duration);
    }
}
