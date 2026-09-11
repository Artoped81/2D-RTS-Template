using System.Collections.Generic;
using RTSTemplate.Core;
using RTSTemplate.Data;

namespace RTSTemplate.Simulation
{
    public class Faction
    {
        public int Id { get; }
        public float MoveSpeedMultiplier { get; private set; } = 1f;

        private readonly Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
        private readonly HashSet<UpgradeDefinition> researchedUpgrades = new HashSet<UpgradeDefinition>();

        public Faction(int id)
        {
            Id = id;
        }

        public bool HasResearched(UpgradeDefinition upgrade) => researchedUpgrades.Contains(upgrade);

        public void ApplyUpgrade(UpgradeDefinition upgrade)
        {
            researchedUpgrades.Add(upgrade);
            MoveSpeedMultiplier *= upgrade.MoveSpeedMultiplier;
        }

        public int GetResource(ResourceType type) => resources.TryGetValue(type, out var amount) ? amount : 0;

        public void Add(ResourceType type, int amount)
        {
            resources[type] = GetResource(type) + amount;
        }

        public bool CanAfford(IEnumerable<ResourceCost> costs)
        {
            foreach (var cost in costs)
                if (GetResource(cost.Type) < cost.Amount)
                    return false;
            return true;
        }

        public bool TrySpend(IEnumerable<ResourceCost> costs)
        {
            if (!CanAfford(costs)) return false;

            foreach (var cost in costs)
                resources[cost.Type] = GetResource(cost.Type) - cost.Amount;
            return true;
        }
    }
}
