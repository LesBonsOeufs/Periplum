using UnityEngine;
using DG.Tweening;

namespace Com.GabrielBernabeu.Common
{
    public class PassiveRotate : MonoBehaviour
    {
        [SerializeField] private float loopDuration = 2f;
        
        private Quaternion initRotation;

        // Start is called before the first frame update
        private void Awake()
        {
            initRotation = transform.rotation;
        }

        private void OnEnable()
        {
            transform.rotation = initRotation;

            DOTween.Sequence(transform).SetLoops(-1)
                .Append(transform.DOBlendableRotateBy(new Vector3(0f, 360f, 0f), loopDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear))
                .SetUpdate(true);
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);
        }
    }
}
