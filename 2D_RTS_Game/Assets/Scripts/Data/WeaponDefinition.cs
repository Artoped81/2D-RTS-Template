using UnityEngine;

namespace RTSTemplate.Data
{
    [CreateAssetMenu(fileName = "NewWeaponDefinition", menuName = "RTS Template/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        public string DisplayName = "New Weapon";

        public int Damage = 5;
        public float Range = 3f;
        public int RateOfFireTicks = 20;
    }
}
