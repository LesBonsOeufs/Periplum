using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Periplum
{
    public class MapPlayer : MonoBehaviour
    {
        public const float STEPS_PER_UNIT = 200f;

        [SerializeField] private LineRenderer pathRenderer;
        [SerializeField] private TextMeshProUGUI remainingMetersTmp;

        private List<Vector3> path;

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

        private void Awake()
        {
            remainingMetersTmp.gameObject.SetActive(false);
            Pedometer.Instance.OnStepsUpdate += Pedometer_OnStepsUpdate;
        }

        private void Pedometer_OnStepsUpdate(Pedometer sender)
        {
            ProgressOnPath(sender.StepsCountSinceLast / STEPS_PER_UNIT);
        }

        private void Start()
        {
            CurrentTile = MapTileManager.Instance.GetTileFromPos(transform.position);

            if (LocalDataSaver<LocalMapData>.CheckIfSaveExists())
            {
                LocalMapData lData = LocalDataSaver<LocalMapData>.CurrentData;
                transform.position = lData.position;
                SetCurrentPath(lData.target);
            }
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
                SetCurrentPath(lMouseWorldPos);
            }
        }

        private void SetCurrentPath(Vector3 target)
        {
            List<Vector3> lPath = MapTileManager.Instance.FindPath(transform.position, target);

            if (lPath == null)
            {
                return;
            }
            else if (lPath.Count == 1)
            {
                //Already on target tile, cancel current path
                if (transform.position == lPath[0])
                    path = null;
                //Partially on target tile
                else
                    lPath.Insert(0, transform.position);
            }
            else
            {
                if (transform.position.IsOnLine(lPath[0], lPath[1]))
                    lPath[0] = transform.position;
                else
                    lPath.Insert(0, transform.position);
            }

            path = lPath;
            RefreshFromPath();
        }

        [Button]
        private void Test() => ProgressOnPath(10f * Time.deltaTime);
        private void ProgressOnPath(float progression)
        {
            if (path == null || path.Count == 1)
                return;

            float lDistanceForNextPoint;
            Vector3 lToNextPoint;

            while (progression > 0)
            {
                lToNextPoint = path[1] - path[0];
                lDistanceForNextPoint = lToNextPoint.magnitude;
                lDistanceForNextPoint -= progression;

                //Point not passed.
                if (lDistanceForNextPoint > 0f)
                {
                    path[0] = path[1] - lToNextPoint.normalized * lDistanceForNextPoint;
                    progression = 0;
                }
                //Point passed.
                else
                {
                    path.RemoveAt(0);
                    progression = Mathf.Abs(lDistanceForNextPoint);
                }

                //Finished path
                if (path.Count == 1)
                {
                    //OnCompletePath?.Invoke();
                    break;
                }
            }

            RefreshFromPath();
        }

        private void RefreshFromPath()
        {
            pathRenderer.positionCount = path.Count;
            pathRenderer.SetPositions(path.ToArray());
            transform.position = path[0];

            int lTotalPathStepsDistance = Mathf.CeilToInt(GetTotalPathDistance() * STEPS_PER_UNIT);
            remainingMetersTmp.gameObject.SetActive(lTotalPathStepsDistance > 0f);
            remainingMetersTmp.text = $"{Mathf.CeilToInt(lTotalPathStepsDistance / Pedometer.STEPS_PER_METERS)} meters remaining";

            LocalDataSaver<LocalMapData>.CurrentData.position = path[0];
            LocalDataSaver<LocalMapData>.CurrentData.target = path[^1];
            LocalDataSaver<LocalMapData>.SaveCurrentData();

            if (lTotalPathStepsDistance > 0f)
                Pedometer.Instance.StartStepsTracker(lTotalPathStepsDistance);
        }

        private float GetTotalPathDistance()
        {
            int lPathCount = path.Count;
            float lTotalPathDistance = 0f;

            for (int i = 0; i < lPathCount - 1; i++)
                lTotalPathDistance += (path[i + 1] - path[i]).magnitude;

            return lTotalPathDistance;
        }
    }
}