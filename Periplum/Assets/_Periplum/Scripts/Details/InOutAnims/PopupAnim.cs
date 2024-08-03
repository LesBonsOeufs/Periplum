using DG.Tweening;
using UnityEngine;

public class PopupAnim : MonoBehaviour, IInOutAnim
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 0f;

    private float initEulerX;

    private void Awake()
    {
        Vector3 lEulerRotation = transform.localEulerAngles;
        initEulerX = lEulerRotation.x;
        lEulerRotation.x = -90f;
        transform.localEulerAngles = lEulerRotation;
    }

    public void In()
    {
        Vector3 lEulerRotation = transform.localEulerAngles;
        lEulerRotation.x = initEulerX;

        DOTween.Sequence(this)
            .AppendInterval(delay)
            .Append(transform.DOLocalRotate(lEulerRotation, duration)
                .SetEase(Ease.OutBack));
    }

    public void Out()
    {
        Vector3 lEulerRotation = transform.localEulerAngles;
        lEulerRotation.x = -90f;
        transform.DOLocalRotate(lEulerRotation, duration);
    }
}