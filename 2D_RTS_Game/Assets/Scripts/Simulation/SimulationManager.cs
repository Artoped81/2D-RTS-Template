using System.Collections.Generic;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class SimulationManager : MonoBehaviour
    {
        [Tooltip("Simulation steps per second, independent of render frame rate.")]
        [SerializeField] private int ticksPerSecond = 20;

        [SerializeField] private int currentTick;
        public int CurrentTick => currentTick;

        public TerrainMap ActiveMap { get; set; }
        public List<Unit> Units { get; } = new List<Unit>();
        public List<Building> Buildings { get; } = new List<Building>();
        public Faction PlayerFaction { get; private set; }

        public event System.Action<Unit> UnitSpawned;
        public event System.Action<Unit> UnitDied;

        private readonly Dictionary<Vector2Int, Unit> unitOccupancy = new Dictionary<Vector2Int, Unit>();
        private int nextFactionId = 1;

        private float tickInterval;
        private float accumulator;

        private void Awake()
        {
            tickInterval = 1f / ticksPerSecond;
            PlayerFaction = new Faction(0);
        }

        public Faction CreateFaction() => new Faction(nextFactionId++);

        public bool IsCellOccupiedByUnit(Vector2Int cell) => unitOccupancy.ContainsKey(cell);

        public void RegisterUnit(Unit unit)
        {
            Units.Add(unit);
            unitOccupancy[unit.GridPosition] = unit;
        }

        private void UnregisterUnit(Unit unit)
        {
            if (unitOccupancy.TryGetValue(unit.GridPosition, out var occupant) && occupant == unit)
                unitOccupancy.Remove(unit.GridPosition);
        }

        public void NotifyUnitMoved(Unit unit, Vector2Int previousCell, Vector2Int newCell)
        {
            if (unitOccupancy.TryGetValue(previousCell, out var occupant) && occupant == unit)
                unitOccupancy.Remove(previousCell);
            unitOccupancy[newCell] = unit;
        }

        public Building FindNearestDropOff(Faction faction, Vector2Int from)
        {
            Building nearest = null;
            int bestDistance = int.MaxValue;

            foreach (var building in Buildings)
            {
                if (building.Owner != faction || !building.IsComplete || !building.Definition.AcceptsResourceDeposits)
                    continue;

                int distance = Mathf.Abs(building.Origin.x - from.x) + Mathf.Abs(building.Origin.y - from.y);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    nearest = building;
                }
            }

            return nearest;
        }

        private void Update()
        {
            accumulator += Time.deltaTime;
            while (accumulator >= tickInterval)
            {
                accumulator -= tickInterval;
                RunTick();
            }
        }

        private void RunTick()
        {
            currentTick++;

            if (ActiveMap == null) return;

            foreach (var unit in Units)
            {
                unit.TickMove(ActiveMap, this, tickInterval);
                unit.TickGather(ActiveMap, this, tickInterval);
                unit.TickCombat(ActiveMap, this);
            }

            for (int i = Units.Count - 1; i >= 0; i--)
            {
                var unit = Units[i];
                if (!unit.IsDead) continue;

                Units.RemoveAt(i);
                UnregisterUnit(unit);
                UnitDied?.Invoke(unit);
            }

            foreach (var building in Buildings)
            {
                building.TickConstruction();

                var producedUnit = building.TickProduction(ActiveMap, this);
                if (producedUnit != null)
                {
                    RegisterUnit(producedUnit);
                    UnitSpawned?.Invoke(producedUnit);
                }

                var completedUpgrade = building.TickResearch();
                if (completedUpgrade != null)
                    building.Owner.ApplyUpgrade(completedUpgrade);
            }
        }
    }
}
