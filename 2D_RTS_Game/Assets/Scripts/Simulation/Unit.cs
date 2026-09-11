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

        public void MoveTo(TerrainMap map, SimulationManager sim, Vector2Int targetCell)
        {
            gatherPhase = GatherPhase.None;
            MoveToInternal(map, sim, targetCell);
        }

        public void StartGathering(TerrainMap map, SimulationManager sim, ResourceNode node, Building dropOffBuilding)
        {
            gatherNode = node;
            dropOff = dropOffBuilding;
            gatherPhase = GatherPhase.ToNode;
            MoveTowardNode(map, sim);
        }

        public void TickMove(TerrainMap map, SimulationManager sim, float tickDeltaTime)
        {
            if (path == null || path.Count == 0) return;

            float speed = Definition.MoveSpeed * (Owner?.MoveSpeedMultiplier ?? 1f);
            moveProgress += speed * tickDeltaTime;
            while (moveProgress >= 1f && path.Count > 0)
            {
                var next = path.Peek();
                if (!map.IsPassable(Definition.Domain, next.x, next.y) ||
                    (sim != null && sim.IsCellOccupiedByUnit(next)))
                {
                    path = null;
                    moveProgress = 0f;
                    return;
                }

                var previous = GridPosition;
                path.Dequeue();
                GridPosition = next;
                moveProgress -= 1f;

                sim?.NotifyUnitMoved(this, previous, next);
            }
        }

        public void TickGather(TerrainMap map, SimulationManager sim, float tickDeltaTime)
        {
            switch (gatherPhase)
            {
                case GatherPhase.None:
                    return;

                case GatherPhase.ToNode:
                    if (IsMoving) break;
                    if (IsAdjacentTo(gatherNode.Position))
                    {
                        gatherPhase = GatherPhase.Harvesting;
                        gatherProgress = 0f;
                    }
                    else
                    {
                        MoveTowardNode(map, sim);
                    }
                    break;

                case GatherPhase.Harvesting:
                    gatherProgress += tickDeltaTime;
                    if (gatherProgress >= Definition.GatherTimePerLoad)
                    {
                        carriedAmount = gatherNode.Harvest(Definition.CarryCapacity);
                        carriedType = gatherNode.Type;
                        gatherPhase = GatherPhase.ToDropoff;
                        MoveTowardDropoff(map, sim);
                    }
                    break;

                case GatherPhase.ToDropoff:
                    if (IsMoving) break;
                    if (IsAdjacentToBuilding(dropOff))
                    {
                        Owner?.Add(carriedType, carriedAmount);
                        carriedAmount = 0;

                        if (gatherNode.AmountRemaining > 0)
                        {
                            gatherPhase = GatherPhase.ToNode;
                            MoveTowardNode(map, sim);
                        }
                        else
                        {
                            gatherPhase = GatherPhase.None;
                        }
                    }
                    else
                    {
                        MoveTowardDropoff(map, sim);
                    }
                    break;
            }
        }

        private void MoveTowardNode(TerrainMap map, SimulationManager sim)
        {
            var candidates = AdjacencyUtil.GetPerimeterCells(gatherNode.Position, Vector2Int.one);
            var cell = AdjacencyUtil.FindFreeCell(candidates, map, Definition.Domain, sim, GridPosition);
            if (cell.HasValue) MoveToInternal(map, sim, cell.Value);
        }

        private void MoveTowardDropoff(TerrainMap map, SimulationManager sim)
        {
            var cell = AdjacencyUtil.FindFreeCell(dropOff.GetAdjacentCells(), map, Definition.Domain, sim, GridPosition);
            if (cell.HasValue) MoveToInternal(map, sim, cell.Value);
        }

        private bool IsAdjacentTo(Vector2Int cell)
        {
            return Mathf.Abs(GridPosition.x - cell.x) <= 1 && Mathf.Abs(GridPosition.y - cell.y) <= 1;
        }

        private bool IsAdjacentToBuilding(Building building)
        {
            foreach (var cell in building.GetAdjacentCells())
                if (GridPosition == cell)
                    return true;
            return false;
        }

        private void MoveToInternal(TerrainMap map, SimulationManager sim, Vector2Int targetCell)
        {
            var found = GridPathfinder.FindPath(map, Definition.Domain, GridPosition, targetCell, sim);
            path = found != null ? new Queue<Vector2Int>(found) : null;
            moveProgress = 0f;
        }
    }
}
