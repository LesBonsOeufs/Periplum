using System;
using UnityEngine;

public class MapZoomManager : Singleton<MapZoomManager>
{
    [NonSerialized] public float targetedZoom = 0f;

    private Vector3 zoomTargetPos;
    private Vector3 cameraBasePos;
    private float cameraBaseSize;
    private float smoothDampVelocity;

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
            lMainCamera.transform.position = Vector3.Lerp(cameraBasePos, zoomTargetPos, Zoom);
            Camera.main.orthographicSize = Mathf.Lerp(cameraBaseSize, .25f, Zoom);
        }
    }
    private float _zoom;

    protected override void Awake()
    {
        base.Awake();
        cameraBaseSize = Camera.main.orthographicSize;
    }

    private void Update()
    {
        Zoom = Mathf.SmoothDamp(_zoom, targetedZoom, ref smoothDampVelocity, 0.05f);
    }

    public void SetZoomTargetPos(Vector3 pos)
    {
        cameraBasePos = Camera.main.transform.position;
        zoomTargetPos = pos;
        zoomTargetPos.z = cameraBasePos.z;
    }
}