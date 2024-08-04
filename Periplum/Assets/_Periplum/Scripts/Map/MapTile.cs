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

        [field: SerializeField, ReadOnly] public TimedLine TimedLine { get; private set; }
        
        private IInOutAnim[] detailableElements;

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
                MapZoomManager.Instance.SetZoomTargetPos(transform.position);
            else
            {
                if (MapZoomManager.Instance.targetedZoom > ZoomHandler.ZOOM_CAP)
                {
                    ZoomToDetails();
                    return;
                }
                else
                    MapZoomManager.Instance.targetedZoom = 0f;
            }

            OnZoomActive?.Invoke(active);
        }

        private void PinchDetector_OnPinchValueUpdate(float value)
        {
            MapZoomManager.Instance.targetedZoom = value;
        }

        public void SetTimedLine(TimedLine timedLine)
        {
            TimedLine = timedLine;
        }

        private void ZoomToDetails()
        {
            //Stop interactions
            IsDetailable = false;
            DetailsContentSpawner.detailsPrefab = info.DetailsPrefab;

            DOVirtual.Float(MapZoomManager.Instance.targetedZoom, 1f, .25f, (value) => MapZoomManager.Instance.targetedZoom = value)
                .OnComplete(() => SceneManager.LoadScene(detailsScene));
        }

        private void OnValidate()
        {
            if (info == null)
                return;

            icon.sprite = info.Icon;
        }
    }
}