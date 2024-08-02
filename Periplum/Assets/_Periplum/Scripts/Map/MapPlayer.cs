using NaughtyAttributes;
using System;
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

        private List<Vector3> path;
        private MapTile currentTile;
        private TimedLineData currentTimedLineData = null;

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
                {
                    currentTimedLineData = null;
                    currentTile.OnZoomActive += CurrentTile_OnZoomActive;
                }
                else
                    currentTile.OnZoomActive -= CurrentTile_OnZoomActive;
            }
        }

        private bool _isPathComplete;

        private void Awake()
        {
            Pedometer.Instance.OnStepsUpdate += Pedometer_OnStepsUpdate;
        }

        private void Start()
        {
            if (LocalDataSaver<LocalMapData>.CheckIfSaveExists())
            {
                LocalMapData lData = LocalDataSaver<LocalMapData>.CurrentData;
                currentTimedLineData = lData.timedLineData;
                transform.position = lData.position;

                if (currentTimedLineData != null)
                    InitTimedLinePath(lData.target);
                else
                    InitPath(lData.target);
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
            //Block path choice if committed on a timedLine
            if (!(currentTimedLineData != null && currentTile == null) &&
                Pointer.current.press.wasPressedThisFrame)
            {
                Camera lCamera = Camera.main;
                float lDepthFromCamera = Vector3.Project(transform.position - lCamera.transform.position, lCamera.transform.forward).magnitude;
                Vector3 lMouseScreenPos = Pointer.current.position.ReadValue();
                lMouseScreenPos.z = lDepthFromCamera;
                Vector3 lMouseWorldPos = Camera.main.ScreenToWorldPoint(lMouseScreenPos);
                ContextBasedInitPath(lMouseWorldPos);
            }
        }

        private void ContextBasedInitPath(Vector3 worldPosTarget)
        {
            //Check if a timedLine joins current & target tiles
            if (currentTimedLineData == null &&
                currentTile != null && currentTile.TimedLine != null &&
                currentTile.TimedLine.GetOther(currentTile, out MapTile lOtherTile) &&
                lOtherTile == MapTileManager.Instance.GetTileFromPos(worldPosTarget))
            {
                currentTimedLineData = new TimedLineData(
                    DateTime.Now.AddMinutes(currentTile.TimedLine.NMinutesLimit),
                    currentTile.transform.position);

                InitTimedLinePath(lOtherTile.transform.position);
            }
            else
                InitPath(worldPosTarget);
        }

        private void InitTimedLinePath(Vector3 worldPosTarget)
        {
            path = new List<Vector3>() { transform.position, worldPosTarget };
            RefreshFromPath();
        }

        private void InitPath(Vector3 worldPosTarget)
        {
            List<Vector3> lPath = MapTileManager.Instance.FindPath(transform.position, worldPosTarget);

            if (lPath == null)
                return;

            currentTimedLineData = null;

            //Only partially on target tile
            if (lPath.Count == 1)
            {
                if (transform.position != lPath[0])
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

            while (progression > 0 && path.Count > 1)
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
            }

            RefreshFromPath();
        }

        private void RefreshFromPath()
        {
            //Reset player to timedLine start
            if (currentTimedLineData?.timeLimit <= DateTime.Now)
            {
                path = new List<Vector3>() { currentTimedLineData.startPosition };
                currentTimedLineData = null;
            }

            pathRenderer.positionCount = path.Count;
            pathRenderer.SetPositions(path.ToArray());
            transform.position = path[0];
            TrySetCurrentTile();

            int lTotalPathStepsDistance = Mathf.CeilToInt(GetTotalPathDistance() * STEPS_PER_UNIT);
            IsPathComplete = lTotalPathStepsDistance == 0;
            remainingMetersTmp.text = $"{lTotalPathStepsDistance} steps remaining";

            LocalDataSaver<LocalMapData>.CurrentData.timedLineData = currentTimedLineData;
            LocalDataSaver<LocalMapData>.CurrentData.position = path[0];
            LocalDataSaver<LocalMapData>.CurrentData.target = path[^1];
            LocalDataSaver<LocalMapData>.SaveCurrentData();

            if (!IsPathComplete)
                Pedometer.Instance.StartStepsTracker(lTotalPathStepsDistance, currentTimedLineData?.timeLimit);
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

        /// <summary>
        /// Current tile is only set if player is exactly on its position
        /// </summary>
        private void TrySetCurrentTile()
        {
            MapTile lTile = MapTileManager.Instance.GetTileFromPos(transform.position);

            if (lTile != null && transform.position == lTile.transform.position)
                currentTile = lTile;
            else
                currentTile = null;
        }
    }
}