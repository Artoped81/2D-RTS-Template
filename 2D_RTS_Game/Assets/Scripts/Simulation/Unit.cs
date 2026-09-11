using System.Collections.Generic;
using RTSTemplate.Data;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class Unit
    {
        public UnitDefinition Definition { get; }
        public Vector2Int GridPosition { get; private set; }
        public int CurrentHealth { get; private set; }

        private Queue<Vector2Int> path;
        private float moveProgress;

        public Unit(UnitDefinition definition, Vector2Int startCell)
        {
            Definition = definition;
            CurrentHealth = definition.MaxHealth;
            GridPosition = startCell;
        }

        public void MoveTo(TerrainMap map, Vector2Int targetCell)
        {
            var found = GridPathfinder.FindPath(map, Definition.Domain, GridPosition, targetCell);
            path = found != null ? new Queue<Vector2Int>(found) : null;
            moveProgress = 0f;
        }

        public void TickMove(TerrainMap map, float tickDeltaTime)
        {
            if (path == null || path.Count == 0) return;

            moveProgress += Definition.MoveSpeed * tickDeltaTime;
            while (moveProgress >= 1f && path.Count > 0)
            {
                var next = path.Peek();
                if (!map.IsPassable(Definition.Domain, next.x, next.y))
                {
                    path = null;
                    moveProgress = 0f;
                    return;
                }

                path.Dequeue();
                GridPosition = next;
                moveProgress -= 1f;
            }
        }
    }
}
