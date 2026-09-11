using UnityEngine;

namespace RTSTemplate.Rendering
{
    public static class BuildingPlaceholderSprite
    {
        private static Sprite cached;

        public static Sprite Get()
        {
            if (cached != null) return cached;

            const int size = 8;
            var texture = new Texture2D(size, size) { filterMode = FilterMode.Point };

            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            texture.SetPixels(pixels);
            texture.Apply();

            cached = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return cached;
        }
    }
}
