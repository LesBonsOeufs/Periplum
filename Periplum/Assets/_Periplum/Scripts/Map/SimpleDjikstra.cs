using System;
using System.Collections.Generic;
using UnityEngine;

public static class SimpleDjikstra
{
    public static List<Vector2Int> Execute(Vector2Int origin, Vector2Int target, Func<Vector2Int, Vector2Int[]> getNeighborsFunc, Func<Vector2Int, bool> isWalkableFunc)
    {
        List<PathTile> lTestedTiles = new() { new PathTile(origin, null) };
        List<PathTile> lNextTilesToTest = new();
        List<Vector2Int> lBannedPos = new();

        PathTile lPathTile;

        while (lTestedTiles.Count > 0)
        {
            for (int i = lTestedTiles.Count - 1; i >= 0; i--)
            {
                lPathTile = lTestedTiles[i];

                if (lPathTile.position == target)
                    return lPathTile.GetPath();

                foreach (Vector2Int lNeighbor in getNeighborsFunc(lPathTile.position))
                {
                    if (!lBannedPos.Contains(lNeighbor))
                        lNextTilesToTest.Add(new PathTile(lNeighbor, lPathTile));
                }

                lTestedTiles.Remove(lPathTile);
                lBannedPos.Add(lPathTile.position);
            }

            lTestedTiles = new(lNextTilesToTest);
            lNextTilesToTest.Clear();
        }

        return null;
    }

    private class PathTile
    {
        public Vector2Int position;
        public PathTile previousTile;

        public PathTile(Vector2Int position, PathTile previousTile)
        {
            this.position = position;
            this.previousTile = previousTile;
        }

        public List<Vector2Int> GetPath()
        {
            List<Vector2Int> lPath = new ();

            PathTile lPathTile = this;

            while (lPathTile != null)
            {
                lPath.Add(lPathTile.position);
                lPathTile = lPathTile.previousTile;
            }

            lPath.Reverse();
            return lPath;
        }
    }
}
