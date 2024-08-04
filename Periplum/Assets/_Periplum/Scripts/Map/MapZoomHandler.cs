using UnityEngine;

public class MapZoomHandler : ZoomHandler
{
    //Quick singleton
    public static MapZoomHandler Instance { get; private set; }

    private Vector3 zoomTargetPos;
    private Vector3 cameraBasePos;
    private float cameraBaseSize;

    public override float Zoom 
    { 
        get => base.Zoom;

        protected set
        {
            if (Zoom == value)
                return;
            else if (value < 0.00001f)
                value = 0f;

            base.Zoom = value;
            Camera lMainCamera = Camera.main;
            lMainCamera.transform.position = Vector3.Lerp(cameraBasePos, zoomTargetPos, Zoom);
            Camera.main.orthographicSize = Mathf.Lerp(cameraBaseSize, .25f, Zoom);
        }
    }

    protected void Awake()
    {
        Instance = this;
        cameraBaseSize = Camera.main.orthographicSize;
    }

    public void SetZoomTargetPos(Vector3 pos)
    {
        cameraBasePos = Camera.main.transform.position;
        zoomTargetPos = pos;
        zoomTargetPos.z = cameraBasePos.z;
    }
}