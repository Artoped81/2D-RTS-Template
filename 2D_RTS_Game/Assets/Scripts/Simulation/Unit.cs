using System.Collections.Generic;
using RTSTemplate.Core;
using RTSTemplate.Data;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class Unit
    {
        private enum GatherPhase
        {
            None,
            ToNode,
            Harvesting,
            ToDropoff
        }

        public UnitDefinition Definition { get; }
        public Vector2Int GridPosition { get; private set; }
        public int CurrentHealth { get; private set; }
        public Faction Owner { get; set; }
        public bool IsMoving => path != null && path.Count > 0;

        private Queue<Vector2Int> path;
        private float moveProgress;

        private GatherPhase gatherPhase = GatherPhase.None;
        private ResourceNode gatherNode;
        private Building dropOff;
        private float gatherProgress;
        private int carriedAmount;
        private ResourceType carriedType;

        public Unit(UnitDefinition definition, Vector2Int startCell)
        {
            Definition = definition;
            CurrentHealth = definition.MaxHealth;
            GridPosition = startCell;
        }

        public void MoveTo(TerrainMap map, Vector2Int targetCell)
        {
            gatherPhase = GatherPhase.None;
            var found = GridPathfinder.FindPath(map, Definition.Domain, GridPosition, targetCell);
            path = found != null ? new Queue<Vector2Int>(found) : null;
            moveProgress = 0f;
        }

        public void StartGathering(TerrainMap map, ResourceNode node, Building dropOffBuilding)
        {
            gatherNode = node;
            dropOff = dropOffBuilding;
            gatherPhase = GatherPhase.ToNode;
            MoveToInternal(map, node.Position);
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

        public void TickGather(TerrainMap map, float tickDeltaTime)
        {
            switch (gatherPhase)
            {
                case GatherPhase.None:
                    return;

                case GatherPhase.ToNode:
                    if (!IsMoving && GridPosition == gatherNode.Position)
                    {
                        gatherPhase = GatherPhase.Harvesting;
                        gatherProgress = 0f;
                    }
                    break;

                case GatherPhase.Harvesting:
                    gatherProgress += tickDeltaTime;
                    if (gatherProgress >= Definition.GatherTimePerLoad)
                    {
                        carriedAmount = gatherNode.Harvest(Definition.CarryCapacity);
                        carriedType = gatherNode.Type;
                        gatherPhase = GatherPhase.ToDropoff;
                        MoveToInternal(map, dropOff.EntrancePosition);
                    }
                    break;

                case GatherPhase.ToDropoff:
                    if (!IsMoving && GridPosition == dropOff.EntrancePosition)
                    {
                        Owner?.Add(carriedType, carriedAmount);
                        carriedAmount = 0;

                        if (gatherNode.AmountRemaining > 0)
                        {
                            gatherPhase = GatherPhase.ToNode;
                            MoveToInternal(map, gatherNode.Position);
                        }
                        else
                        {
                            gatherPhase = GatherPhase.None;
                        }
                    }
                    break;
            }
        }

        private void MoveToInternal(TerrainMap map, Vector2Int targetCell)
        {
            var found = GridPathfinder.FindPath(map, Definition.Domain, GridPosition, targetCell);
            path = found != null ? new Queue<Vector2Int>(found) : null;
            moveProgress = 0f;
        }
    }
}
