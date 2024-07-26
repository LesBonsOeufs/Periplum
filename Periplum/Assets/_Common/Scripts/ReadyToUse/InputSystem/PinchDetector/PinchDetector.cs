using System;
using System.Collections;
using UnityEngine;

namespace Periplum
{
    public class PinchDetector : Singleton<PinchDetector>
    {
        private PinchControls controls;
        private Coroutine zoomCoroutine;

        public event Action OnPinchStart;
        public event Action<float> OnPinchDistanceUpdate;
        public event Action OnPinchStop;

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
            OnPinchStart?.Invoke();
            zoomCoroutine = StartCoroutine(ZoomDetection());
        }

        private void StopZoom()
        {
            StopCoroutine(zoomCoroutine);
            OnPinchStop?.Invoke();
        }

        private IEnumerator ZoomDetection()
        {
            float lPreviousDistance = 0f;
            float lDistance = 0f;

            while (true)
            {
                lDistance = Vector2.Distance(controls.Actions.PrimaryFingerPosition.ReadValue<Vector2>(),
                    controls.Actions.SecondaryFingerPosition.ReadValue<Vector2>());

                OnPinchDistanceUpdate?.Invoke(lDistance);

                //Zoom out
                if (lDistance > lPreviousDistance)
                {
                    
                }
                //Zoom in
                else if (lDistance < lPreviousDistance)
                {

                }

                lPreviousDistance = lDistance;
                yield return null;
            }
        }
    }
}
