using RTSTemplate.Data;
using RTSTemplate.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTemplate.Rendering
{
    public class ProductionController : MonoBehaviour
    {
        [SerializeField] private EconomyBootstrap economy;
        [SerializeField] private UnitDefinition producibleUnit;
        [SerializeField] private UpgradeDefinition testUpgrade;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || economy?.TownHall == null) return;

            if (keyboard.pKey.wasPressedThisFrame)
                economy.TownHall.TryEnqueueProduction(producibleUnit);

            if (keyboard.uKey.wasPressedThisFrame)
                economy.TownHall.TryStartResearch(testUpgrade);
        }
    }
}
