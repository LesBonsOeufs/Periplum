using DG.Tweening;
using UnityEngine;

namespace Com.GabrielBernabeu.Common
{
    public class PassiveScale : MonoBehaviour
    {
        [SerializeField] private Vector3 maxAddedScale = Vector3.zero;
        [SerializeField] private float loopDuration = 2f;

        private Sequence scaleLoop;

        private void OnEnable()
        {
            scaleLoop = DOTween.Sequence(transform).SetLoops(-1)
                .Append(transform.DOBlendableScaleBy(maxAddedScale, loopDuration * 0.5f).SetEase(Ease.InOutSine))
                .Append(transform.DOBlendableScaleBy(-maxAddedScale, loopDuration * 0.5f).SetEase(Ease.InOutSine)).SetUpdate(true);
        }

        private void OnDisable()
        {
            scaleLoop.Kill(true);
        }
    }
}