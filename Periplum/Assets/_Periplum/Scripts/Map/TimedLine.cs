using NaughtyAttributes;
using UnityEngine;

namespace Periplum
{
    public class TimedLine : MonoBehaviour
    {
        [SerializeField] private MapTile startTile;
        [SerializeField] private MapTile endTile;

        [field: SerializeField] public int NMinutesLimit { get; private set; } = 15;
        [ShowNativeProperty] public int StepsDistance { get; private set; }

        /// <returns>Returns null if given tile is not start or end</returns>
        public MapTile GetOther(MapTile tile)
        {
            if (tile == startTile)
                return endTile;
            else if (tile == endTile)
                return startTile;
            else
                return null;
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

            StepsDistance = 
                Mathf.CeilToInt((endTile.transform.position - startTile.transform.position).magnitude * MapPlayer.STEPS_PER_UNIT);
        }
    }
}
