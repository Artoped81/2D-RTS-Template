using RTSTemplate.Simulation;
using UnityEngine;

namespace RTSTemplate.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BuildingView : MonoBehaviour
    {
        public Building Building { get; private set; }

        private SpriteRenderer spriteRenderer;

        public static BuildingView Spawn(Building building)
        {
            if (building.Definition.ViewPrefab == null)
            {
                Debug.LogWarning($"{building.Definition.DisplayName} has no ViewPrefab assigned.");
                return null;
            }

            var view = Object.Instantiate(building.Definition.ViewPrefab).GetComponent<BuildingView>();
            view.Bind(building);
            return view;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Bind(Building building)
        {
            Building = building;

            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = building.Definition.PlaceholderSprite != null
                    ? building.Definition.PlaceholderSprite
                    : BuildingPlaceholderSprite.Get();
            }

            var footprint = building.Definition.FootprintSize;
            transform.localScale = new Vector3(footprint.x, footprint.y, 1f);
            transform.position = new Vector3(
                building.Origin.x + footprint.x / 2f,
                building.Origin.y + footprint.y / 2f,
                0f);
        }

        private void LateUpdate()
        {
            if (Building == null) return;
            spriteRenderer.color = Building.IsComplete ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
