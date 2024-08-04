using System;
using UnityEngine;

public abstract class ZoomHandler : MonoBehaviour
{
    public const float ZOOM_CAP = 0.25f;

    [SerializeField] private MonoBehaviour[] deactivateForZoom;

    [NonSerialized] public float targetedZoom = 0f;
    private float smoothDampVelocity;

    public virtual float Zoom
    {
        get => _zoom;

        protected set
        {
            _zoom = value;

            if (deactivateForZoom != null) 
            {
                foreach (MonoBehaviour lBehaviour in deactivateForZoom)
                    lBehaviour.enabled = _zoom == 0f;
            }
        }
    }
    private float _zoom;

    private void Update()
    {
        Zoom = Mathf.SmoothDamp(_zoom, targetedZoom, ref smoothDampVelocity, 0.05f);
    }
}