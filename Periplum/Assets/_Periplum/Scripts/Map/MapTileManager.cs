using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Periplum
{
    [RequireComponent(typeof(Grid))]
    public class MapTileManager : Singleton<MapTileManager>
    {
        private readonly Vector2Int[] EVEN_NEIGHBOR_DIRECTIONS = new Vector2Int[]
        { 
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
        };

        private readonly Vector2Int[] ODD_NEIGHBOR_DIRECTIONS = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
        };

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

        /// <returns>Null if no tile on position</returns>
        public MapTile GetTileFromPos(Vector3 pos)
        {
            catalog.TryGetValue(grid.WorldToCell(pos), out MapTile lMapTile);
            return lMapTile;
        }

        public List<Vector3> FindPath(Vector3 origin, Vector3 target)
        {
            Vector2Int lOriginCellPos = (Vector2Int)grid.WorldToCell(origin);
            Vector2Int lTargetCellPos = (Vector2Int)grid.WorldToCell(target);

            List<Vector2Int> lCellPath = SimpleDjikstra.Execute(lOriginCellPos, lTargetCellPos, GetNeighbors, TestWalkable);
            List<Vector3> lWorldPath = new ();

            if (lCellPath == null)
                return null;

            foreach (Vector2Int lCellPos in lCellPath)
                lWorldPath.Add(grid.CellToWorld((Vector3Int)lCellPos));

            return lWorldPath;
        }

        private Vector2Int[] GetNeighbors(Vector2Int position)
        {
            List<Vector2Int> lNeighbors = new ();
            Vector2Int[] lNeighborPositions = GetNeighboringDirections(position);

            foreach (Vector2Int lCellPos in lNeighborPositions)
                lNeighbors.Add(lCellPos);

            return lNeighbors.ToArray();
        }

        private bool TestWalkable(Vector2Int position)
        {
            return catalog.TryGetValue((Vector3Int)position, out MapTile lMapTile);
            //Could add isWalkable test here
        }

        private Vector2Int[] GetNeighboringDirections(Vector2Int pos)
        {
            bool lIsEven = pos.y % 2 == 0;
            Vector2Int[] lUsedArray = lIsEven ? EVEN_NEIGHBOR_DIRECTIONS : ODD_NEIGHBOR_DIRECTIONS;

            int lDirectionsLength = lUsedArray.Length;
            Vector2Int[] lReturnedValue = new Vector2Int[lDirectionsLength];
            Array.Copy(lUsedArray, lReturnedValue, lDirectionsLength);

            for (int i = 0; i < lDirectionsLength; i++)
                lReturnedValue[i] += pos;

            return lReturnedValue;
        }
    }
}