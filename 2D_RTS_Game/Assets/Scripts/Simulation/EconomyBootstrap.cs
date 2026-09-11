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
        [SerializeField] private GameObject goldNodeViewPrefab;
        [SerializeField] private Vector2Int woodNodePosition = new Vector2Int(16, 2);
        [SerializeField] private int woodNodeAmount = 500;
        [SerializeField] private GameObject woodNodeViewPrefab;

        public List<ResourceNode> ResourceNodes { get; } = new List<ResourceNode>();
        public Building TownHall { get; private set; }

        public void Setup(TerrainMap map)
        {
            TownHall = BuildingPlacer.PlaceFree(map, simulationManager.PlayerFaction, townHallDefinition, townHallOrigin);
            TownHall.CompleteImmediately();
            simulationManager.Buildings.Add(TownHall);
            BuildingView.Spawn(TownHall);

            var goldNode = new ResourceNode(goldNodePosition, ResourceType.Gold, goldNodeAmount);
            var woodNode = new ResourceNode(woodNodePosition, ResourceType.Wood, woodNodeAmount);
            ResourceNodes.Add(goldNode);
            ResourceNodes.Add(woodNode);
            ResourceNodeView.Spawn(goldNode, goldNodeViewPrefab);
            ResourceNodeView.Spawn(woodNode, woodNodeViewPrefab);
        }

        public ResourceNode FindNodeAt(Vector2Int cell)
        {
            foreach (var node in ResourceNodes)
                if (node.Position == cell)
                    return node;
            return null;
        }
    }
}
