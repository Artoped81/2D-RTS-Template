using RTSTemplate.Core;
using UnityEngine;

namespace RTSTemplate.Data
{
    [CreateAssetMenu(fileName = "NewUnitDefinition", menuName = "RTS Template/Unit Definition")]
    public class UnitDefinition : ScriptableObject
    {
        public string DisplayName = "New Unit";
        public Sprite PlaceholderSprite;

        public MovementDomain Domain = MovementDomain.Land;
        public float MoveSpeed = 2f;

        public int MaxHealth = 50;
        public WeaponDefinition Weapon;

        [Header("Gathering")]
        public bool CanGather;
        public int CarryCapacity = 10;
        public float GatherTimePerLoad = 2f;

        [Header("Production")]
        public ResourceCost[] Costs = new ResourceCost[0];
        public int ProductionTimeTicks = 60;
    }
}
