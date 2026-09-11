using System.Collections.Generic;
using RTSTemplate.Core;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public static class AdjacencyUtil
    {
        public static List<Vector2Int> GetPerimeterCells(Vector2Int origin, Vector2Int size)
        {
            var cells = new List<Vector2Int>();

            for (int x = -1; x <= size.x; x++)
            {
                cells.Add(origin + new Vector2Int(x, -1));
                cells.Add(origin + new Vector2Int(x, size.y));
            }

            for (int y = 0; y < size.y; y++)
            {
                cells.Add(origin + new Vector2Int(-1, y));
                cells.Add(origin + new Vector2Int(size.x, y));
            }

            return cells;
        }

        public static Vector2Int? FindFreeCell(List<Vector2Int> candidates, TerrainMap map, MovementDomain domain, SimulationManager sim, Vector2Int from)
        {
            Vector2Int? best = null;
            int bestDistance = int.MaxValue;

            foreach (var cell in candidates)
            {
                if (!map.IsPassable(domain, cell.x, cell.y)) continue;
                if (sim != null && sim.IsCellOccupiedByUnit(cell)) continue;

                int distance = Mathf.Abs(cell.x - from.x) + Mathf.Abs(cell.y - from.y);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = cell;
                }
            }

            return best;
        }
    }
}
