using System;
using System.Collections;
using UnityEngine;

namespace Periplum
{
    public class PinchDetector : Singleton<PinchDetector>
    {
        private PinchControls controls;
        private Coroutine zoomCoroutine;

        public event Action<bool> OnPinchActive;
        public event Action<float> OnPinchDistanceUpdate;

        protected override void Awake()
        {
            base.Awake();
            controls = new PinchControls();
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private void Start()
        {
            controls.Actions.SecondaryTouchContact.started += _ => StartZoom();
            controls.Actions.SecondaryTouchContact.canceled += _ => StopZoom();
        }

        private void StartZoom()
        {
            OnPinchActive?.Invoke(true);
            zoomCoroutine = StartCoroutine(ZoomDetection());
        }

        private void StopZoom()
        {
            StopCoroutine(zoomCoroutine);
            OnPinchActive?.Invoke(false);
        }

        private IEnumerator ZoomDetection()
        {
            float lDistance;

            while (true)
            {
                lDistance = Vector2.Distance(controls.Actions.PrimaryFingerPosition.ReadValue<Vector2>(),
                    controls.Actions.SecondaryFingerPosition.ReadValue<Vector2>());

                OnPinchDistanceUpdate?.Invoke(lDistance);
                yield return null;
            }
        }
    }
}
