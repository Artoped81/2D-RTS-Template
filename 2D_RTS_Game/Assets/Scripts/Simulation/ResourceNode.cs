using RTSTemplate.Core;
using UnityEngine;

namespace RTSTemplate.Simulation
{
    public class ResourceNode
    {
        public Vector2Int Position { get; }
        public ResourceType Type { get; }
        public int AmountRemaining { get; private set; }

        public ResourceNode(Vector2Int position, ResourceType type, int amount)
        {
            Position = position;
            Type = type;
            AmountRemaining = amount;
        }

        public int Harvest(int requestedAmount)
        {
            int amount = Mathf.Min(requestedAmount, AmountRemaining);
            AmountRemaining -= amount;
            return amount;
        }
    }
}
