using RTSTemplate.Core;
using UnityEngine;

namespace RTSTemplate.Data
{
    [CreateAssetMenu(fileName = "NewBuildingDefinition", menuName = "RTS Template/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        public string DisplayName = "New Building";
        public Sprite PlaceholderSprite;

        public MovementDomain Domain = MovementDomain.Land;
        public Vector2Int FootprintSize = Vector2Int.one;

        public int MaxHealth = 200;
    }
}
