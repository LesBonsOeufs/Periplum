using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Periplum
{
    public class MapPlayer : MonoBehaviour
    {
        [SerializeField] private LineRenderer pathRenderer;

        public MapTile CurrentTile
        {
            get => _currentTile;

            set
            { 
                _currentTile = value;
                transform.position = _currentTile.transform.position;
            }
        }
        private MapTile _currentTile;

        private void Start()
        {
            CurrentTile = MapTileManager.Instance.GetTileFromPos(transform.position);
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Camera lCamera = Camera.main;
                float lDepthFromCamera = Vector3.Project(transform.position - lCamera.transform.position, lCamera.transform.forward).magnitude;
                Vector3 lMouseScreenPos = Mouse.current.position.ReadValue();
                lMouseScreenPos.z = lDepthFromCamera;
                Vector3 lMouseWorldPos = Camera.main.ScreenToWorldPoint(lMouseScreenPos);

                List<Vector3> lPath = MapTileManager.Instance.Dijkstra(transform.position, lMouseWorldPos);
                int lPathCount = lPath.Count;
                pathRenderer.positionCount = lPathCount;
                pathRenderer.SetPositions(lPath.ToArray());
            }
        }
    }
}