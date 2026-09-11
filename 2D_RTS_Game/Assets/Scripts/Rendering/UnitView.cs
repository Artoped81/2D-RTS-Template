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

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Bind(Unit unit)
        {
            Unit = unit;
            spriteRenderer.sprite = unit.Definition.PlaceholderSprite != null
                ? unit.Definition.PlaceholderSprite
                : UnitPlaceholderSprite.Get();
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
