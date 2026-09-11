using UnityEngine;

namespace RTSTemplate.Data
{
    [CreateAssetMenu(fileName = "NewUpgradeDefinition", menuName = "RTS Template/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        public string DisplayName = "New Upgrade";
        public int DurationTicks = 200;
    }
}
