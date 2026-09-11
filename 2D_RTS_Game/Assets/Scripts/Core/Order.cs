using UnityEngine;

namespace RTSTemplate.Core
{
    [System.Serializable]
    public class Order
    {
        public OrderType Type;
        public Vector2Int TargetCell;
        public int TargetEntityId = -1;

        public int Stage;
        public int TotalStages;

        public Order(OrderType type)
        {
            Type = type;
        }

        public bool IsComplete => TotalStages > 0 && Stage >= TotalStages;
    }
}
