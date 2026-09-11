using RTSTemplate.Data;
using RTSTemplate.Rendering;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private UnitDefinition testUnitDefinition;
        [SerializeField] private int unitCount = 4;

        private void OnEnable()
        {
            if (simulationManager != null) simulationManager.UnitSpawned += OnUnitSpawned;
        }

        private void OnDisable()
        {
            if (simulationManager != null) simulationManager.UnitSpawned -= OnUnitSpawned;
        }

        public void SpawnInitialUnits(TerrainMap map)
        {
            if (testUnitDefinition == null) return;

            int spawned = 0;
            for (int y = 0; y < map.Height && spawned < unitCount; y++)
            {
                for (int x = 0; x < map.Width && spawned < unitCount; x++)
                {
                    if (!map.IsPassable(testUnitDefinition.Domain, x, y)) continue;

                    var unit = new Unit(testUnitDefinition, new Vector2Int(x, y))
                    {
                        Owner = simulationManager.PlayerFaction
                    };
                    simulationManager.RegisterUnit(unit);
                    UnitView.Spawn(unit);
                    spawned++;
                }
            }
        }

        private static void OnUnitSpawned(Unit unit) => UnitView.Spawn(unit);
    }
}
