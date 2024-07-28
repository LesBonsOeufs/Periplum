using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Periplum
{
    public class ZoomHandler : MonoBehaviour
    {
        public const float ZOOM_CAP = 0.35f;

        [SerializeField, Scene] private int mapScene;

        private void Start()
        {
            PinchDetector.Instance.OnPinchValueUpdate += PinchDetector_OnPinchValueUpdate;
        }

        private void PinchDetector_OnPinchValueUpdate(float value)
        {
            if (value < -ZOOM_CAP)
                SceneManager.LoadScene(mapScene);
        }
    }
}
