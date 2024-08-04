using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ScaleOnTap : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private float scale = -0.1f;
    [SerializeField] private UnityEvent onTap;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * scale, .5f);
        onTap?.Invoke();
    }
}