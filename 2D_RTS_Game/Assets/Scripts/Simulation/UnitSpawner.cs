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

        public void SpawnInitialUnits(TerrainMap map)
        {
            if (testUnitDefinition == null) return;

            int spawned = 0;
            for (int y = 0; y < map.Height && spawned < unitCount; y++)
            {
                for (int x = 0; x < map.Width && spawned < unitCount; x++)
                {
                    if (!map.IsPassable(testUnitDefinition.Domain, x, y)) continue;

                    var unit = new Unit(testUnitDefinition, new Vector2Int(x, y));
                    simulationManager.Units.Add(unit);
                    SpawnView(unit);
                    spawned++;
                }
            }
        }

        private static void SpawnView(Unit unit)
        {
            var go = new GameObject(unit.Definition.DisplayName);
            go.AddComponent<SpriteRenderer>();
            var view = go.AddComponent<UnitView>();
            view.Bind(unit);
        }
    }
}
