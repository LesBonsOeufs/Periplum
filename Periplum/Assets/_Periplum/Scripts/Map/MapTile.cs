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
        [SerializeField] private Transform detailablesContainer;

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
            Instantiate(info.TileDetailablesPrefab, detailablesContainer);
            detailableElements = detailablesContainer.GetComponentsInChildren<IInOutAnim>(true);
        }

        private void PinchDetector_OnPinchActive(bool active)
        {
            if (active)
                MapZoomHandler.Instance.SetZoomTargetPos(transform.position);
            else
            {
                if (MapZoomHandler.Instance.targetedZoom > MapZoomHandler.ZOOM_CAP)
                {
                    ZoomToDetails();
                    return;
                }

                MapZoomHandler.Instance.targetedZoom = 0f;
            }

            OnZoomActive?.Invoke(active);
        }

        private void PinchDetector_OnPinchValueUpdate(float value)
        {
            MapZoomHandler.Instance.targetedZoom = value;
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

            DOVirtual.Float(MapZoomHandler.Instance.targetedZoom, 1f, .25f, (value) => MapZoomHandler.Instance.targetedZoom = value)
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