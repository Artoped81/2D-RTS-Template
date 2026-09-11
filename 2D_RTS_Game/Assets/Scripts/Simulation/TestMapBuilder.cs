using RTSTemplate.Core;
using RTSTemplate.Rendering;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class TestMapBuilder : MonoBehaviour
    {
        [SerializeField] private int width = 20;
        [SerializeField] private int height = 20;
        [SerializeField] private TerrainMapRenderer renderer;
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private UnitSpawner unitSpawner;
        [SerializeField] private EconomyBootstrap economyBootstrap;

        public TerrainMap Map { get; private set; }

        private void Start()
        {
            Map = new TerrainMap(width, height);
            CarveTestLake();
            renderer.Render(Map);
            simulationManager.ActiveMap = Map;
            economyBootstrap.Setup(Map);
            unitSpawner.SpawnInitialUnits(Map);
        }

        private void CarveTestLake()
        {
            int lakeMinX = width / 2 - 3;
            int lakeMaxX = width / 2 + 2;
            int lakeMinY = height / 2 - 3;
            int lakeMaxY = height / 2 + 2;

            for (int y = lakeMinY; y <= lakeMaxY; y++)
            {
                for (int x = lakeMinX; x <= lakeMaxX; x++)
                {
                    if (Map.IsInBounds(x, y))
                        Map.SetDomain(x, y, MovementDomain.Water);
                }
            }
        }
    }
}
