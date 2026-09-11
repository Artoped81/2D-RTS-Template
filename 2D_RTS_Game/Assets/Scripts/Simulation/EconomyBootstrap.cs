using System.Collections.Generic;
using RTSTemplate.Core;
using RTSTemplate.Data;
using RTSTemplate.Rendering;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class EconomyBootstrap : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private BuildingDefinition townHallDefinition;
        [SerializeField] private Vector2Int townHallOrigin = new Vector2Int(2, 2);
        [SerializeField] private Vector2Int goldNodePosition = new Vector2Int(2, 6);
        [SerializeField] private int goldNodeAmount = 500;
        [SerializeField] private Vector2Int woodNodePosition = new Vector2Int(16, 2);
        [SerializeField] private int woodNodeAmount = 500;

        public List<ResourceNode> ResourceNodes { get; } = new List<ResourceNode>();
        public Building TownHall { get; private set; }

        public void Setup(TerrainMap map)
        {
            TownHall = BuildingPlacer.PlaceFree(map, simulationManager.PlayerFaction, townHallDefinition, townHallOrigin);
            TownHall.CompleteImmediately();
            simulationManager.Buildings.Add(TownHall);
            SpawnBuildingView(TownHall);

            var goldNode = new ResourceNode(goldNodePosition, ResourceType.Gold, goldNodeAmount);
            var woodNode = new ResourceNode(woodNodePosition, ResourceType.Wood, woodNodeAmount);
            ResourceNodes.Add(goldNode);
            ResourceNodes.Add(woodNode);
            SpawnResourceNodeView(goldNode);
            SpawnResourceNodeView(woodNode);
        }

        public ResourceNode FindNodeAt(Vector2Int cell)
        {
            foreach (var node in ResourceNodes)
                if (node.Position == cell)
                    return node;
            return null;
        }

        private static void SpawnBuildingView(Building building)
        {
            var go = new GameObject(building.Definition.DisplayName);
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<BuildingView>().Bind(building);
        }

        private static void SpawnResourceNodeView(ResourceNode node)
        {
            var go = new GameObject(node.Type + " Node");
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<ResourceNodeView>().Bind(node);
        }
    }
}
