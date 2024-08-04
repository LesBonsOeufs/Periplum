using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Periplum
{
    public class ZoomHandler : MonoBehaviour
    {
        public const float ZOOM_CAP = 0.35f;

        [SerializeField, Scene] private int mapScene;
        [SerializeField] private Image faderImage;

        private void Start()
        {
            PinchDetector.Instance.OnPinchValueUpdate += PinchDetector_OnPinchValueUpdate;
        }

        private void PinchDetector_OnPinchValueUpdate(float value)
        {
            float lZoomLerp = Mathf.InverseLerp(0f, ZOOM_CAP, value);
            Color lColor = faderImage.color;
            lColor.a = lZoomLerp;
            faderImage.color = lColor;
            Camera.main.fieldOfView = Mathf.Lerp(60f, 100f, lZoomLerp);

            if (value < -ZOOM_CAP)
                SceneManager.LoadScene(mapScene);
        }
    }
}
