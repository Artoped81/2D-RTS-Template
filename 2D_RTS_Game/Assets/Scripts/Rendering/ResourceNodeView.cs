using RTSTemplate.Core;
using RTSTemplate.Simulation;
using UnityEngine;

namespace RTSTemplate.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ResourceNodeView : MonoBehaviour
    {
        private static readonly Color GoldColor = new Color(1f, 0.85f, 0.2f);
        private static readonly Color WoodColor = new Color(0.55f, 0.35f, 0.15f);

        private SpriteRenderer spriteRenderer;

        public static ResourceNodeView Spawn(ResourceNode node, GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"No view prefab assigned for {node.Type} node.");
                return null;
            }

            var view = Object.Instantiate(prefab).GetComponent<ResourceNodeView>();
            view.Bind(node);
            return view;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Bind(ResourceNode node)
        {
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = UnitPlaceholderSprite.Get();
                spriteRenderer.color = node.Type == ResourceType.Gold ? GoldColor : WoodColor;
            }

            transform.localScale = Vector3.one * 0.8f;
            transform.position = new Vector3(node.Position.x + 0.5f, node.Position.y + 0.5f, 0f);
        }
    }
}
