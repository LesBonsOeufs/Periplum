using UnityEngine;

namespace Periplum
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private MapTileInfo info;
        [SerializeField] private SpriteRenderer icon;

        private void OnValidate()
        {
            if (info == null)
                return;

            icon.sprite = info.Icon;
        }
    }
}