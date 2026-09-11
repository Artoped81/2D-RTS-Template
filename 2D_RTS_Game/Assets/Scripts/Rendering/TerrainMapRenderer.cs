using RTSTemplate.Core;
using RTSTemplate.Simulation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RTSTemplate.Rendering
{
    public class TerrainMapRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Color landColor = new Color(0.35f, 0.55f, 0.25f);
        [SerializeField] private Color waterColor = new Color(0.2f, 0.4f, 0.75f);

        private Tile landTile;
        private Tile waterTile;

        public void Render(TerrainMap map)
        {
            if (landTile == null) landTile = PlaceholderTileFactory.CreateSolidColorTile(landColor);
            if (waterTile == null) waterTile = PlaceholderTileFactory.CreateSolidColorTile(waterColor);

            tilemap.ClearAllTiles();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var cell = map.GetCell(x, y);
                    var tile = cell.Domain == MovementDomain.Water ? waterTile : landTile;
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
}
