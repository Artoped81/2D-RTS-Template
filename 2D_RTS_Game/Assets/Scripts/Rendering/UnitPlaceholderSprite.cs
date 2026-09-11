using UnityEngine;

namespace RTSTemplate.Rendering
{
    public static class UnitPlaceholderSprite
    {
        private static Sprite cached;

        public static Sprite Get()
        {
            if (cached != null) return cached;

            const int size = 24;
            var texture = new Texture2D(size, size) { filterMode = FilterMode.Point };
            var center = new Vector2(size / 2f, size / 2f);
            var radius = size / 2f - 1f;

            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool inside = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) <= radius;
                    pixels[y * size + x] = inside ? Color.white : new Color(0, 0, 0, 0);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            cached = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return cached;
        }
    }
}
