using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Periplum
{
    public class DetailsZoomHandler : ZoomHandler
    {
        [SerializeField, Scene] private int mapScene;
        [SerializeField] private Image faderImage;

        public override float Zoom 
        { 
            get => base.Zoom;

            protected set
            {
                if (value > 0f)
                    value = 0f;

                base.Zoom = value;
                Color lColor = faderImage.color;
                lColor.a = -Zoom;
                faderImage.color = lColor;
                Camera.main.fieldOfView = Mathf.Lerp(60f, 100f, -Zoom);
            }
        }

        private void Start()
        {
            PinchDetector.Instance.OnPinchActive += PinchDetector_OnPinchActive;
            PinchDetector.Instance.OnPinchValueUpdate += PinchDetector_OnPinchValueUpdate;
        }

        private void PinchDetector_OnPinchActive(bool active)
        {
            if (!active)
            {
                if (targetedZoom < -MapZoomHandler.ZOOM_CAP)
                {
                    ZoomToMap();
                    return;
                }

                targetedZoom = 0f;
            }
        }

        private void PinchDetector_OnPinchValueUpdate(float value)
        {
            targetedZoom = value;
        }

        private void ZoomToMap()
        {
            DOVirtual.Float(targetedZoom, -1f, .25f, value => targetedZoom = value)
                .OnComplete(() => SceneManager.LoadScene(mapScene));
        }
    }
}