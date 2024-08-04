using UnityEngine;
using DG.Tweening;

public abstract class FadeAnim : MonoBehaviour, IInOutAnim
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float delay = 0f;

    public abstract float Alpha { get; protected set; }

    protected virtual void Awake()
    {
        Alpha = 0f;
    }

    public void In()
    {
        DOTween.Sequence(this)
            .AppendInterval(delay)
            .Append(DOVirtual.Float(Alpha, 1f, duration, value => Alpha = value));
    }

    public void Out()
    {
        DOVirtual.Float(Alpha, 0f, duration, value => Alpha = value);
    }
}