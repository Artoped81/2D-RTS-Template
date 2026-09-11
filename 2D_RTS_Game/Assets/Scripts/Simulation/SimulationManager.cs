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

        private float tickInterval;
        private float accumulator;

        private void Awake()
        {
            tickInterval = 1f / ticksPerSecond;
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
                unit.TickMove(ActiveMap, tickInterval);
        }
    }
}
