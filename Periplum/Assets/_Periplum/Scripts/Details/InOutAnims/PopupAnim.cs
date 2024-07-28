using DG.Tweening;
using UnityEngine;

public class PopupAnim : MonoBehaviour, IInOutAnim
{
    [SerializeField] private float duration = 1f;
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
        transform.DOLocalRotate(lEulerRotation, duration)
            .SetEase(Ease.OutBack);
    }

    public void Out()
    {
        Vector3 lEulerRotation = transform.localEulerAngles;
        lEulerRotation.x = -90f;
        transform.DOLocalRotate(lEulerRotation, duration);
    }
}