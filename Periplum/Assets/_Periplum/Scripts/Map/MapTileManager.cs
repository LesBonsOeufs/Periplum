using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

namespace Periplum
{
    [RequireComponent(typeof(Grid))]
    public class MapTileManager : Singleton<MapTileManager>
    {
        [SerializeField, ReadOnly] private SerializedDictionary<Vector3Int, MapTile> catalog = new ();

        private Grid grid;

        protected override void Awake()
        {
            base.Awake();

            grid = GetComponent<Grid>();
            MapTile[] lMapTiles = FindObjectsOfType<MapTile>();

            foreach (MapTile lMapTile in lMapTiles)
                catalog.Add(grid.WorldToCell(lMapTile.transform.position), lMapTile);
        }

        public MapTile GetTileFromPos(Vector3 pos)
        {
            return catalog[grid.WorldToCell(pos)];
        }
    }
}