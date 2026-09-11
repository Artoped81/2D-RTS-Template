using RTSTemplate.Data;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public static class BuildingPlacer
    {
        public static bool CanPlace(TerrainMap map, BuildingDefinition definition, Vector2Int origin)
        {
            for (int y = 0; y < definition.FootprintSize.y; y++)
                for (int x = 0; x < definition.FootprintSize.x; x++)
                    if (!map.IsPassable(definition.Domain, origin.x + x, origin.y + y))
                        return false;
            return true;
        }

        public static Building PlaceFree(TerrainMap map, Faction owner, BuildingDefinition definition, Vector2Int origin)
        {
            var building = new Building(definition, origin, owner);
            MarkFootprint(map, definition, origin);
            return building;
        }

        public static bool TryPlacePaid(TerrainMap map, Faction faction, BuildingDefinition definition, Vector2Int origin, out Building building)
        {
            building = null;
            if (!CanPlace(map, definition, origin)) return false;
            if (!faction.TrySpend(definition.Costs)) return false;

            building = new Building(definition, origin, faction);
            MarkFootprint(map, definition, origin);
            return true;
        }

        private static void MarkFootprint(TerrainMap map, BuildingDefinition definition, Vector2Int origin)
        {
            for (int y = 0; y < definition.FootprintSize.y; y++)
                for (int x = 0; x < definition.FootprintSize.x; x++)
                    map.SetOccupied(origin.x + x, origin.y + y, true);
        }
    }
}
