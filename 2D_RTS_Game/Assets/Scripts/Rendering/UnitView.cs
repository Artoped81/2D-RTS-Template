using RTSTemplate.Simulation;
using UnityEngine;

namespace RTSTemplate.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UnitView : MonoBehaviour
    {
        public Unit Unit { get; private set; }

        private SpriteRenderer spriteRenderer;
        private static readonly Color SelectedTint = new Color(1f, 1f, 0.5f);

        public static UnitView Spawn(Unit unit)
        {
            if (unit.Definition.ViewPrefab == null)
            {
                Debug.LogWarning($"{unit.Definition.DisplayName} has no ViewPrefab assigned.");
                return null;
            }

            var view = Object.Instantiate(unit.Definition.ViewPrefab).GetComponent<UnitView>();
            view.Bind(unit);
            return view;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Bind(Unit unit)
        {
            Unit = unit;

            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = unit.Definition.PlaceholderSprite != null
                    ? unit.Definition.PlaceholderSprite
                    : UnitPlaceholderSprite.Get();
            }

            SyncPosition();
        }

        public void SetSelected(bool selected)
        {
            spriteRenderer.color = selected ? SelectedTint : Color.white;
        }

        private void LateUpdate()
        {
            SyncPosition();
        }

        private void SyncPosition()
        {
            if (Unit == null) return;
            transform.position = new Vector3(Unit.GridPosition.x + 0.5f, Unit.GridPosition.y + 0.5f, 0f);
        }
    }
}
