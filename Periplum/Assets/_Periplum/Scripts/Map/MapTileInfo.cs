using NaughtyAttributes;
using UnityEngine;

namespace Periplum
{
    [CreateAssetMenu(fileName = nameof(MapTileInfo), menuName = "Periplum/" + nameof(MapTileInfo))]
    public class MapTileInfo : ScriptableObject
    {
        [field: SerializeField, ShowAssetPreview] public Sprite Icon { get; private set; }
        [field: SerializeField] public DetailsContent DetailsPrefab { get; private set; }
    }
}