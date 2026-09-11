using RTSTemplate.Core;
using RTSTemplate.Simulation;
using TMPro;
using UnityEngine;

namespace RTSTemplate.UI
{
    public class ResourceHud : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private TMP_Text goldLabel;
        [SerializeField] private TMP_Text woodLabel;

        private void Update()
        {
            if (simulationManager?.PlayerFaction == null) return;

            if (goldLabel != null)
                goldLabel.text = $"Gold: {simulationManager.PlayerFaction.GetResource(ResourceType.Gold)}";

            if (woodLabel != null)
                woodLabel.text = $"Wood: {simulationManager.PlayerFaction.GetResource(ResourceType.Wood)}";
        }
    }
}
