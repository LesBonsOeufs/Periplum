using UnityEngine;
using UnityEngine.Events;

namespace Periplum
{
    public class Unlockable : MonoBehaviour
    {
        [SerializeField] private UnityEvent onUnlocked;
        private bool isUnlocked = false;

        private void Awake()
        {
            gameObject.SetActive(isUnlocked);
        }

        public void Unlock()
        {
            isUnlocked = true;
            onUnlocked?.Invoke();
            gameObject.SetActive(isUnlocked);
        }
    }
}
