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
        public Vector2Int EntrancePosition => new Vector2Int(Origin.x, Origin.y - 1);

        public Building(BuildingDefinition definition, Vector2Int origin, Faction owner)
        {
            Definition = definition;
            Origin = origin;
            Owner = owner;
            CurrentHealth = definition.MaxHealth;
        }

        public void TickConstruction()
        {
            if (IsComplete) return;
            ConstructionProgress++;
        }

        public void CompleteImmediately()
        {
            ConstructionProgress = Definition.ConstructionTimeTicks;
        }
    }
}
