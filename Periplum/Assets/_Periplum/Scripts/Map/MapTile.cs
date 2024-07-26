using DG.Tweening;
using System;
using UnityEngine;

namespace Periplum
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private MapTileInfo info;
        [SerializeField] private SpriteRenderer tile;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color detailableColor;

        private float smoothDampVelocity;
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

                if (_zoom == 0f)
                {
                    OnZoomActive?.Invoke(true);
                    cameraBasePos = Camera.main.transform.position;
                    tileCenteredCameraPos = transform.position;
                    tileCenteredCameraPos.z = cameraBasePos.z;
                    cameraBaseSize = Camera.main.orthographicSize;
                }

                _zoom = value;

                if (_zoom == 0f)
                    OnZoomActive?.Invoke(false);

                Camera lMainCamera = Camera.main;
                lMainCamera.transform.position = Vector3.Lerp(cameraBasePos, tileCenteredCameraPos, Zoom);
                Camera.main.orthographicSize = Mathf.Lerp(cameraBaseSize, 0.5f, Zoom);
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
                tile.DOColor(_isDetailable ? detailableColor : Color.white, 0.25f);

                if (_isDetailable)
                    PinchDetector.Instance.OnPinchDistanceUpdate += PinchDetector_OnPinchDistanceUpdate;
                else
                    PinchDetector.Instance.OnPinchDistanceUpdate -= PinchDetector_OnPinchDistanceUpdate;
            }
        }
        private bool _isDetailable;

        public event Action<bool> OnZoomActive;

        private void PinchDetector_OnPinchDistanceUpdate(float obj)
        {
            Zoom = Mathf.SmoothDamp(_zoom, obj, ref smoothDampVelocity, 3f);

            //if (_zoom > zoomCap)
            //{
            //    //Big zoom anim + stop listening to pinch
            //}
        }

        private void OnValidate()
        {
            if (info == null)
                return;

            icon.sprite = info.Icon;
        }
    }
}