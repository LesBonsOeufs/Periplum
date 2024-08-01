using UnityEngine;

namespace Periplum
{
    public class TimedLine : MonoBehaviour
    {
        [field: SerializeField] public MapTile StartTile { get; private set; }
        [field: SerializeField] public MapTile EndTile { get; private set; }

        private void OnValidate()
        {
            if (StartTile == null || EndTile == null)
                return;

            StartTile.SetTimedLine(this);
            EndTile.SetTimedLine(this);

            Vector3 lStartPos = StartTile.transform.position;
            Vector3 lEndPos = EndTile.transform.position;
            Vector3 lStartToEnd = lEndPos - lStartPos;

            transform.SetPositionAndRotation(
                (lStartPos + lEndPos) * 0.5f, 
                Quaternion.FromToRotation(Vector3.right, lStartToEnd));

            transform.localScale = new Vector3(lStartToEnd.magnitude, transform.localScale.y, transform.localScale.z);
        }
    }
}
