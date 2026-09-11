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
        public bool IsDead => CurrentHealth <= 0;

        private Queue<Vector2Int> path;
        private float moveProgress;

        private GatherPhase gatherPhase = GatherPhase.None;
        private ResourceNode gatherNode;
        private Building dropOff;
        private float gatherProgress;
        private int carriedAmount;
        private ResourceType carriedType;

        private Unit attackTarget;
        private int attackCooldown;

        private Vector2Int? moveOrderTarget;

        public Unit(UnitDefinition definition, Vector2Int startCell)
        {
            Definition = definition;
            CurrentHealth = definition.MaxHealth;
            GridPosition = startCell;
        }

        public void MoveTo(TerrainMap map, SimulationManager sim, Vector2Int targetCell)
        {
            gatherPhase = GatherPhase.None;
            attackTarget = null;
            moveOrderTarget = targetCell;
            MoveToInternal(map, sim, targetCell);
        }

        public void Attack(SimulationManager sim, Unit target)
        {
            gatherPhase = GatherPhase.None;
            moveOrderTarget = null;
            attackTarget = target;
            attackCooldown = 0;

            // Stop wherever the unit currently is so TickCombat immediately
            // takes over movement toward the new target, instead of finishing
            // whatever path was already in progress from a prior order.
            path = null;
            moveProgress = 0f;
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        }

        public void TickCombat(TerrainMap map, SimulationManager sim)
        {
            if (attackTarget == null || attackTarget.IsDead || Definition.Weapon == null) return;

            float distance = Vector2.Distance(GridPosition, attackTarget.GridPosition);
            if (distance <= Definition.Weapon.Range)
            {
                path = null;

                if (attackCooldown <= 0)
                {
                    attackTarget.TakeDamage(Definition.Weapon.Damage);
                    attackCooldown = Definition.Weapon.RateOfFireTicks;
                }
                else
                {
                    attackCooldown--;
                }
            }
            else if (!IsMoving)
            {
                // The target's own cell is always occupied by the target itself, so
                // pathing there directly would never succeed - close in on a free cell
                // next to it instead.
                var candidates = AdjacencyUtil.GetPerimeterCells(attackTarget.GridPosition, Vector2Int.one);
                var cell = AdjacencyUtil.FindFreeCell(candidates, map, Definition.Domain, sim, GridPosition);
                if (cell.HasValue) MoveToInternal(map, sim, cell.Value);
            }
        }

        public void StartGathering(TerrainMap map, SimulationManager sim, ResourceNode node, Building dropOffBuilding)
        {
            attackTarget = null;
            moveOrderTarget = null;
            gatherNode = node;
            dropOff = dropOffBuilding;
            gatherPhase = GatherPhase.ToNode;
            MoveTowardNode(map, sim);
        }

        public void TickMove(TerrainMap map, SimulationManager sim, float tickDeltaTime)
        {
            if (path != null && path.Count > 0)
            {
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
                        break;
                    }

                    var previous = GridPosition;
                    path.Dequeue();
                    GridPosition = next;
                    moveProgress -= 1f;

                    sim?.NotifyUnitMoved(this, previous, next);
                }
            }

            if (!moveOrderTarget.HasValue) return;

            if (GridPosition == moveOrderTarget.Value)
            {
                moveOrderTarget = null;
            }
            else if (!IsMoving)
            {
                // A step became blocked (e.g. by another unit passing through) and the
                // path was dropped above. Retry every tick until a path opens up, rather
                // than leaving the unit stranded mid-route.
                MoveToInternal(map, sim, moveOrderTarget.Value);
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
