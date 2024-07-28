using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Periplum
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private MapTileInfo info;
        [SerializeField] private SpriteRenderer tile;
        [SerializeField] private SpriteRenderer icon;

        [Foldout("DetailsView"), SerializeField, Scene] private int detailsScene;
        
        private IInOutAnim[] detailableElements;

        private float smoothDampVelocity;
        private float targetZoom = 0f;

        private Vector3 cameraBasePos;
        private Vector3 tileCenteredCameraPos;
        private float cameraBaseSize;

        public float Zoom
        {
            get => _zoom;

            private set
            {
                if (_zoom == value)
                    return;
                else if (value < 0f)
                    value = 0f;

                _zoom = value;
                Camera lMainCamera = Camera.main;
                lMainCamera.transform.position = Vector3.Lerp(cameraBasePos, tileCenteredCameraPos, Zoom);
                Camera.main.orthographicSize = Mathf.Lerp(cameraBaseSize, .25f, Zoom);
            }
        }
        private float _zoom;

        public bool IsDetailable
        {
            get => _isDetailable;

            set
            {
                if (_isDetailable == value)
                    return;

                _isDetailable = value;

                if (detailableElements != null)
                {
                    foreach (IInOutAnim lElement in detailableElements)
                    {
                        if (_isDetailable)
                            lElement.In();
                        else
                            lElement.Out();
                    }
                }

                if (_isDetailable)
                {
                    PinchDetector.Instance.OnPinchActive += PinchDetector_OnPinchActive;
                    PinchDetector.Instance.OnPinchValueUpdate += PinchDetector_OnPinchValueUpdate;
                }
                else
                {
                    PinchDetector.Instance.OnPinchActive -= PinchDetector_OnPinchActive;
                    PinchDetector.Instance.OnPinchValueUpdate -= PinchDetector_OnPinchValueUpdate;
                }
            }
        }

        private bool _isDetailable;

        public event Action<bool> OnZoomActive;

        private void Awake()
        {
            detailableElements = GetComponentsInChildren<IInOutAnim>(true);
        }

        private void PinchDetector_OnPinchActive(bool active)
        {
            if (active)
            {
                cameraBasePos = Camera.main.transform.position;
                tileCenteredCameraPos = transform.position;
                tileCenteredCameraPos.z = cameraBasePos.z;
                cameraBaseSize = Camera.main.orthographicSize;
            }
            else
            {
                if (targetZoom > ZoomHandler.ZOOM_CAP)
                {
                    ZoomToDetails();
                    return;
                }
                else
                    targetZoom = 0f;
            }

            OnZoomActive?.Invoke(active);
        }

        private void PinchDetector_OnPinchValueUpdate(float value)
        {
            targetZoom = value;
        }

        private void Update()
        {
            Zoom = Mathf.SmoothDamp(_zoom, targetZoom, ref smoothDampVelocity, 0.05f);
        }

        private void ZoomToDetails()
        {
            //Stop interactions
            IsDetailable = false;

            DOVirtual.Float(targetZoom, 1f, .25f, (value) =>
            {
                targetZoom = value;
            }).OnComplete(() =>
            {
                DetailsContentSpawner.detailsPrefab = info.DetailsPrefab;
                SceneManager.LoadScene(detailsScene);
            });
        }

        private void OnValidate()
        {
            if (info == null)
                return;

            icon.sprite = info.Icon;
        }
    }
}