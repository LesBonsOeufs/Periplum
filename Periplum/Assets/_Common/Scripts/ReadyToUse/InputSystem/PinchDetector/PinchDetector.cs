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
        public event Action<float> OnPinchValueUpdate;

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
            float lStartDistance;
            float lDistance;
            Vector2 lScreenSize = new(Screen.width, Screen.height);
            Vector2 lPrimaryPos;
            Vector2 lSecondaryPos;

#if UNITY_EDITOR
            lPrimaryPos = new Vector2(0f, 1f);
#else
            lPrimaryPos = controls.Actions.PrimaryFingerPosition.ReadValue<Vector2>() / lScreenSize;
#endif
            lSecondaryPos = controls.Actions.SecondaryFingerPosition.ReadValue<Vector2>() / lScreenSize;
            lStartDistance = Vector2.Distance(lPrimaryPos, lSecondaryPos);

            while (true)
            {
#if !UNITY_EDITOR
                lPrimaryPos = controls.Actions.PrimaryFingerPosition.ReadValue<Vector2>() / lScreenSize;
#endif
                lSecondaryPos = controls.Actions.SecondaryFingerPosition.ReadValue<Vector2>() / lScreenSize;
                lDistance = Vector2.Distance(lPrimaryPos, lSecondaryPos);

                float lDistanceDelta = lDistance - lStartDistance;
                OnPinchValueUpdate?.Invoke(lDistanceDelta);
                yield return null;
            }
        }
    }
}
