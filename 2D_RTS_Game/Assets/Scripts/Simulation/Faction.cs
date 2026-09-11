using System.Collections.Generic;
using RTSTemplate.Core;

namespace RTSTemplate.Simulation
{
    public class Faction
    {
        public int Id { get; }

        private readonly Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

        public Faction(int id)
        {
            Id = id;
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
