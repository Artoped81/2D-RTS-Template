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

        private float tickInterval;
        private float accumulator;

        private void Awake()
        {
            tickInterval = 1f / ticksPerSecond;
            PlayerFaction = new Faction(0);
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
                unit.TickMove(ActiveMap, tickInterval);
                unit.TickGather(ActiveMap, tickInterval);
            }

            foreach (var building in Buildings)
                building.TickConstruction();
        }
    }
}
