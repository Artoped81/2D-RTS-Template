using RTSTemplate.Data;
using RTSTemplate.Rendering;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private UnitViewRegistry viewRegistry;
        [SerializeField] private UnitDefinition testUnitDefinition;
        [SerializeField] private int unitCount = 4;
        [SerializeField] private bool spawnAsNewFaction;
        [SerializeField] private Vector2Int spawnAreaOrigin = Vector2Int.zero;

        private Faction ownFaction;

        public void SpawnInitialUnits(TerrainMap map)
        {
            if (testUnitDefinition == null) return;

            var faction = ResolveFaction();

            int spawned = 0;
            for (int y = spawnAreaOrigin.y; y < map.Height && spawned < unitCount; y++)
            {
                for (int x = spawnAreaOrigin.x; x < map.Width && spawned < unitCount; x++)
                {
                    if (!map.IsPassable(testUnitDefinition.Domain, x, y)) continue;

                    var unit = new Unit(testUnitDefinition, new Vector2Int(x, y))
                    {
                        Owner = faction
                    };
                    simulationManager.RegisterUnit(unit);
                    viewRegistry.SpawnAndTrack(unit);
                    spawned++;
                }
            }
        }

        private Faction ResolveFaction()
        {
            if (!spawnAsNewFaction) return simulationManager.PlayerFaction;

            if (ownFaction == null) ownFaction = simulationManager.CreateFaction();
            return ownFaction;
        }
    }
}
