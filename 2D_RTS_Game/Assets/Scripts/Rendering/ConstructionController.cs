using RTSTemplate.Data;
using RTSTemplate.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTemplate.Rendering
{
    public class ConstructionController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private BuildingDefinition buildableDefinition;

        private bool placing;

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            if (keyboard.bKey.wasPressedThisFrame) placing = !placing;
            if (!placing) return;

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                placing = false;
                return;
            }

            if (mouse.leftButton.wasPressedThisFrame)
                TryPlaceAt(mouse.position.ReadValue());
        }

        private void TryPlaceAt(Vector2 screenPos)
        {
            if (simulationManager.ActiveMap == null || buildableDefinition == null) return;

            var world = cam.ScreenToWorldPoint(screenPos);
            var origin = new Vector2Int(Mathf.FloorToInt(world.x), Mathf.FloorToInt(world.y));

            if (BuildingPlacer.TryPlacePaid(simulationManager.ActiveMap, simulationManager.PlayerFaction, buildableDefinition, origin, out var building))
            {
                simulationManager.Buildings.Add(building);
                BuildingView.Spawn(building);
                placing = false;
            }
        }
    }
}
