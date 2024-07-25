using System;
using System.Collections.Generic;
using UnityEngine;

public static class SimpleDjikstra
{
    public static List<Vector2Int> Execute(Vector2Int origin, Vector2Int target, Func<Vector2Int, Vector2Int[]> getNeighborsFunc, Func<Vector2Int, bool> isWalkableFunc)
    {
        if (!isWalkableFunc(target))
            return null;

        List<PathTile> lTilesToTest = new() { new PathTile(origin, null) };
        List<PathTile> lNextTilesToTest = new();
        List<Vector2Int> lVisitedTiles = new() { origin };
        PathTile lPathTile;

        while (lTilesToTest.Count > 0)
        {
            for (int i = 0; i < lTilesToTest.Count; i++)
            {
                lPathTile = lTilesToTest[i];

                if (lPathTile.position == target)
                    return lPathTile.GetPath();

                foreach (Vector2Int lNeighbor in getNeighborsFunc(lPathTile.position))
                {
                    if (!lVisitedTiles.Contains(lNeighbor) && isWalkableFunc(lNeighbor))
                    {
                        lNextTilesToTest.Add(new PathTile(lNeighbor, lPathTile));
                        lVisitedTiles.Add(lNeighbor);
                    }
                }
            }

            lTilesToTest = new(lNextTilesToTest);
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
