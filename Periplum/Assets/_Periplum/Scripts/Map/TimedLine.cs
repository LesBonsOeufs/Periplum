using System;
using UnityEngine;

namespace Periplum
{
    public class TimedLine : MonoBehaviour
    {
        [SerializeField] private MapTile startTile;
        [SerializeField] private MapTile endTile;

        [field: SerializeField] public int NMinutesLimit { get; private set; } = 15;

        /// <returns>Returns null if given tile is not start or end</returns>
        public bool GetOther(MapTile tile, out MapTile other)
        {
            if (tile == startTile)
                other = endTile;
            else if (tile == endTile)
                other = startTile;
            else
                other = null;

            return other != null;
        }

        private void OnValidate()
        {
            if (startTile == null || endTile == null)
                return;

            startTile.SetTimedLine(this);
            endTile.SetTimedLine(this);

            Vector3 lStartPos = startTile.transform.position;
            Vector3 lEndPos = endTile.transform.position;
            Vector3 lStartToEnd = lEndPos - lStartPos;

            transform.SetPositionAndRotation(
                (lStartPos + lEndPos) * 0.5f, 
                Quaternion.FromToRotation(Vector3.right, lStartToEnd));

            transform.localScale = new Vector3(lStartToEnd.magnitude, transform.localScale.y, transform.localScale.z);
        }
    }

    [Serializable]
    public class TimedLineData
    {
        public DateTime timeLimit;
        public SerializableVector3 startPosition;

        public TimedLineData(DateTime timeLimit, SerializableVector3 startPosition)
        {
            this.timeLimit = timeLimit;
            this.startPosition = startPosition;
        }
    }
}
