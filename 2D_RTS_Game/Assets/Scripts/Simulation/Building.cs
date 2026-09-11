using System.Collections.Generic;
using RTSTemplate.Data;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class Building
    {
        public BuildingDefinition Definition { get; }
        public Vector2Int Origin { get; }
        public Faction Owner { get; }
        public int CurrentHealth { get; private set; }
        public int ConstructionProgress { get; private set; }

        public bool IsComplete => ConstructionProgress >= Definition.ConstructionTimeTicks;
        public int ProductionQueueLength => productionQueue.Count;
        public UpgradeDefinition ActiveResearch { get; private set; }

        private readonly Queue<UnitDefinition> productionQueue = new Queue<UnitDefinition>();
        private int productionProgress;
        private int researchProgress;

        public Building(BuildingDefinition definition, Vector2Int origin, Faction owner)
        {
            Definition = definition;
            Origin = origin;
            Owner = owner;
            CurrentHealth = definition.MaxHealth;
        }

        public List<Vector2Int> GetAdjacentCells() => AdjacencyUtil.GetPerimeterCells(Origin, Definition.FootprintSize);

        public void TickConstruction()
        {
            if (IsComplete) return;
            ConstructionProgress++;
        }

        public void CompleteImmediately()
        {
            ConstructionProgress = Definition.ConstructionTimeTicks;
        }

        public bool TryEnqueueProduction(UnitDefinition unitDefinition)
        {
            if (!IsComplete || !Owner.TrySpend(unitDefinition.Costs)) return false;

            productionQueue.Enqueue(unitDefinition);
            return true;
        }

        public Unit TickProduction(TerrainMap map, SimulationManager sim)
        {
            if (!IsComplete || productionQueue.Count == 0) return null;

            productionProgress++;
            var current = productionQueue.Peek();
            if (productionProgress < current.ProductionTimeTicks) return null;

            var spawnCell = AdjacencyUtil.FindFreeCell(GetAdjacentCells(), map, current.Domain, sim, Origin);
            if (spawnCell == null) return null;

            productionQueue.Dequeue();
            productionProgress = 0;

            return new Unit(current, spawnCell.Value) { Owner = Owner };
        }

        public bool TryStartResearch(UpgradeDefinition upgrade)
        {
            if (!IsComplete || ActiveResearch != null || !Owner.TrySpend(upgrade.Costs)) return false;

            ActiveResearch = upgrade;
            researchProgress = 0;
            return true;
        }

        public UpgradeDefinition TickResearch()
        {
            if (ActiveResearch == null) return null;

            researchProgress++;
            if (researchProgress < ActiveResearch.DurationTicks) return null;

            var completed = ActiveResearch;
            ActiveResearch = null;
            return completed;
        }
    }
}
