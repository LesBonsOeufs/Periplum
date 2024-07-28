using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Periplum
{
    public class DetailsContent : MonoBehaviour
    {
        [SerializeField] private float startAnimDelay = 0.5f;
        private IInOutAnim[] elements;

        private void Awake()
        {
            elements = GetComponentsInChildren<IInOutAnim>(true);
        }

        private void Start()
        {
            DOVirtual.DelayedCall(startAnimDelay, Show, false);
        }

        [Button]
        public void Show()
        {
            foreach (var lElement in elements)
                lElement.In();
        }

        [Button]
        public void Hide()
        {
            foreach (var lElement in elements)
                lElement.Out();
        }
    }
}
