using DG.Tweening;
using UnityEngine;

namespace Com.GabrielBernabeu.Common
{
    public class PassiveMove : MonoBehaviour
    {
        [SerializeField] private Vector3 maxAddedPosition = Vector3.zero;
        [SerializeField] private float loopDuration = 2f;

        private Sequence moveLoop;

        private void OnEnable()
        {
            moveLoop = DOTween.Sequence(transform).SetLoops(-1)
                .Append(transform.DOBlendableMoveBy(maxAddedPosition, loopDuration * 0.5f).SetEase(Ease.InOutSine))
                .Append(transform.DOBlendableMoveBy(-maxAddedPosition, loopDuration * 0.5f).SetEase(Ease.InOutSine)).SetUpdate(true);
        }

        private void OnDisable()
        {
            moveLoop.Kill(true);
        }
    }
}