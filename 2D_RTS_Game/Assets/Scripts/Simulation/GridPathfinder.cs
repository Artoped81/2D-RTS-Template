using System.Collections.Generic;
using RTSTemplate.Core;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public static class GridPathfinder
    {
        private static readonly Vector2Int[] Neighbors =
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        public static List<Vector2Int> FindPath(TerrainMap map, MovementDomain domain, Vector2Int start, Vector2Int goal)
        {
            if (!map.IsPassable(domain, goal.x, goal.y)) return null;
            if (start == goal) return new List<Vector2Int>();

            var frontier = new Queue<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            frontier.Enqueue(start);
            cameFrom[start] = start;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current == goal) break;

                foreach (var offset in Neighbors)
                {
                    var next = current + offset;
                    if (cameFrom.ContainsKey(next)) continue;
                    if (!map.IsPassable(domain, next.x, next.y)) continue;

                    cameFrom[next] = current;
                    frontier.Enqueue(next);
                }
            }

            if (!cameFrom.ContainsKey(goal)) return null;

            var path = new List<Vector2Int>();
            var step = goal;
            while (step != start)
            {
                path.Add(step);
                step = cameFrom[step];
            }
            path.Reverse();
            return path;
        }
    }
}
