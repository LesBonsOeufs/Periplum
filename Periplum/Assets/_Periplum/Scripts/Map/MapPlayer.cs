using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Periplum
{
    public class MapPlayer : MonoBehaviour
    {
        public const float STEPS_PER_UNIT = 200f;

        [SerializeField] private LineRenderer pathRenderer;
        [SerializeField] private TextMeshProUGUI remainingMetersTmp;

        public bool IsPathComplete
        {
            get => _isPathComplete;

            private set
            {
                if (_isPathComplete == value)
                    return;

                _isPathComplete = value;
                remainingMetersTmp.gameObject.SetActive(!_isPathComplete);
                currentTile.IsDetailable = value;

                if (_isPathComplete)
                    currentTile.OnZoomActive += CurrentTile_OnZoomActive;
                else
                    currentTile.OnZoomActive -= CurrentTile_OnZoomActive;
            }
        }

        private bool _isPathComplete;

        private List<Vector3> path;
        private MapTile currentTile;

        private void Awake()
        {
            Pedometer.Instance.OnStepsUpdate += Pedometer_OnStepsUpdate;
        }

        private void Start()
        {
            if (LocalDataSaver<LocalMapData>.CheckIfSaveExists())
            {
                LocalMapData lData = LocalDataSaver<LocalMapData>.CurrentData;
                transform.position = lData.position;
                SetCurrentPath(lData.target);
            }
        }

        private void Pedometer_OnStepsUpdate(Pedometer sender)
        {
            ProgressOnPath(sender.StepsCountSinceLast / STEPS_PER_UNIT);
        }

        private void CurrentTile_OnZoomActive(bool active)
        {
            gameObject.SetActive(!active);
        }

        private void Update()
        {
            if (Pointer.current.press.wasPressedThisFrame)
            {
                Camera lCamera = Camera.main;
                float lDepthFromCamera = Vector3.Project(transform.position - lCamera.transform.position, lCamera.transform.forward).magnitude;
                Vector3 lMouseScreenPos = Pointer.current.position.ReadValue();
                lMouseScreenPos.z = lDepthFromCamera;
                Vector3 lMouseWorldPos = Camera.main.ScreenToWorldPoint(lMouseScreenPos);
                SetCurrentPath(lMouseWorldPos);
            }
        }

        private void SetCurrentPath(Vector3 worldPosTarget)
        {
            //Check if a timedLine joins current & target tiles
            if (currentTile.TimedLine != null)
            {
                MapTile lOtherTile = currentTile.TimedLine.GetOther(currentTile);

                if (lOtherTile == MapTileManager.Instance.GetTileFromPos(worldPosTarget) == lOtherTile)
                {
                    //LocalDataSaver<LocalMapData>.CurrentData.timedLine = new LocalMapData.TimedLine();
                }
            }

            List<Vector3> lPath = MapTileManager.Instance.FindPath(transform.position, worldPosTarget);

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
        private void PathProgressTest() => ProgressOnPath(0.5f);
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
            currentTile = MapTileManager.Instance.GetTileFromPos(transform.position);
            IsPathComplete = lTotalPathStepsDistance == 0;
            remainingMetersTmp.text = $"{lTotalPathStepsDistance} steps remaining";

            LocalDataSaver<LocalMapData>.CurrentData.position = path[0];
            LocalDataSaver<LocalMapData>.CurrentData.target = path[^1];
            LocalDataSaver<LocalMapData>.SaveCurrentData();

            if (!IsPathComplete)
                Pedometer.Instance.StartStepsTracker(lTotalPathStepsDistance);
            else
                Pedometer.Instance.StopStepsTracker();
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