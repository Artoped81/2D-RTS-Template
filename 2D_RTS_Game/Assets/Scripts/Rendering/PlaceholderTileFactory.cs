using UnityEngine;
using UnityEngine.Tilemaps;

namespace RTSTemplate.Rendering
{
    public static class PlaceholderTileFactory
    {
        public static Tile CreateSolidColorTile(Color color, int pixelsPerCell = 32)
        {
            var texture = new Texture2D(pixelsPerCell, pixelsPerCell)
            {
                filterMode = FilterMode.Point
            };

            var pixels = new Color[pixelsPerCell * pixelsPerCell];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, pixelsPerCell, pixelsPerCell),
                new Vector2(0.5f, 0.5f),
                pixelsPerCell);

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = Color.white;
            return tile;
        }
    }
}
