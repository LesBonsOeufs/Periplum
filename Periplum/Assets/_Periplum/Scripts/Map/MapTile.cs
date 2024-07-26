using DG.Tweening;
using UnityEngine;

namespace Periplum
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private MapTileInfo info;
        [SerializeField] private SpriteRenderer tile;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color detailableColor;

        public void SetDetailable(bool detailable)
        {
            tile.DOColor(detailable ? detailableColor : Color.white, 0.25f);
        }

        private void OnValidate()
        {
            if (info == null)
                return;

            icon.sprite = info.Icon;
        }
    }
}