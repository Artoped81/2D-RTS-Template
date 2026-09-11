using System.Collections.Generic;
using RTSTemplate.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTemplate.Rendering
{
    public class SelectionController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private EconomyBootstrap economy;
        [SerializeField] private float clickTolerance = 0.5f;

        private readonly List<UnitView> selected = new List<UnitView>();
        private Vector2 dragStartScreen;
        private bool dragging;
        private bool hasDragStart;

        private void OnEnable()
        {
            if (simulationManager != null) simulationManager.UnitDied += HandleUnitDied;
        }

        private void OnDisable()
        {
            if (simulationManager != null) simulationManager.UnitDied -= HandleUnitDied;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                dragStartScreen = mouse.position.ReadValue();
                hasDragStart = true;
                dragging = false;
            }

            if (hasDragStart && mouse.leftButton.isPressed)
            {
                var current = mouse.position.ReadValue();
                if ((current - dragStartScreen).sqrMagnitude > 25f) dragging = true;
            }

            if (mouse.leftButton.wasReleasedThisFrame && hasDragStart)
            {
                var endScreen = mouse.position.ReadValue();
                if (dragging) SelectInRect(dragStartScreen, endScreen);
                else SelectSingleAt(endScreen);

                dragging = false;
                hasDragStart = false;
            }

            if (mouse.rightButton.wasPressedThisFrame && selected.Count > 0)
                IssueOrderAt(mouse.position.ReadValue());
        }

        private void OnGUI()
        {
            if (!dragging) return;

            var current = Mouse.current?.position.ReadValue() ?? dragStartScreen;
            GUI.Box(GetGuiRect(dragStartScreen, current), GUIContent.none);
        }

        private static Rect GetGuiRect(Vector2 a, Vector2 b)
        {
            var aGui = new Vector2(a.x, Screen.height - a.y);
            var bGui = new Vector2(b.x, Screen.height - b.y);
            var min = Vector2.Min(aGui, bGui);
            var max = Vector2.Max(aGui, bGui);
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        private void SelectInRect(Vector2 startScreen, Vector2 endScreen)
        {
            ClearSelection();

            var min = Vector2.Min(startScreen, endScreen);
            var max = Vector2.Max(startScreen, endScreen);

            foreach (var view in FindObjectsByType<UnitView>(FindObjectsSortMode.None))
            {
                if (view.Unit.Owner != simulationManager.PlayerFaction) continue;

                var screenPos = cam.WorldToScreenPoint(view.transform.position);
                if (screenPos.x >= min.x && screenPos.x <= max.x && screenPos.y >= min.y && screenPos.y <= max.y)
                    Select(view);
            }
        }

        private void SelectSingleAt(Vector2 screenPos)
        {
            ClearSelection();

            var closest = FindUnitViewAt(screenPos, requireOwnedByPlayer: true);
            if (closest != null) Select(closest);
        }

        private UnitView FindUnitViewAt(Vector2 screenPos, bool requireOwnedByPlayer = false)
        {
            var world = cam.ScreenToWorldPoint(screenPos);
            UnitView closest = null;
            float closestDist = clickTolerance;

            foreach (var view in FindObjectsByType<UnitView>(FindObjectsSortMode.None))
            {
                if (requireOwnedByPlayer && view.Unit.Owner != simulationManager.PlayerFaction) continue;

                float dist = Vector2.Distance(world, view.transform.position);
                if (dist < closestDist)
                {
                    closest = view;
                    closestDist = dist;
                }
            }

            return closest;
        }

        private void IssueOrderAt(Vector2 screenPos)
        {
            if (simulationManager.ActiveMap == null) return;

            var targetView = FindUnitViewAt(screenPos);
            if (targetView != null && targetView.Unit.Owner != simulationManager.PlayerFaction)
            {
                foreach (var view in selected)
                {
                    if (view.Unit.Definition.Weapon != null)
                        view.Unit.Attack(simulationManager, targetView.Unit);
                }
                return;
            }

            var worldTarget = cam.ScreenToWorldPoint(screenPos);
            var targetCell = new Vector2Int(Mathf.FloorToInt(worldTarget.x), Mathf.FloorToInt(worldTarget.y));
            var targetNode = economy != null ? economy.FindNodeAt(targetCell) : null;

            var movers = new List<UnitView>();
            foreach (var view in selected)
            {
                if (targetNode != null && view.Unit.Definition.CanGather)
                {
                    var dropOff = simulationManager.FindNearestDropOff(view.Unit.Owner, view.Unit.GridPosition);
                    if (dropOff != null)
                    {
                        view.Unit.StartGathering(simulationManager.ActiveMap, simulationManager, targetNode, dropOff);
                        continue;
                    }
                }

                movers.Add(view);
            }

            IssueFormationMove(movers, targetCell);
        }

        private void IssueFormationMove(List<UnitView> movers, Vector2Int targetCell)
        {
            int count = movers.Count;
            if (count == 0) return;

            // Re-form into a compact block around the target rather than preserving
            // however spread out the group currently happens to be - otherwise scatter
            // from earlier orders (or combat, gathering, etc.) only ever compounds.
            int gridWidth = Mathf.CeilToInt(Mathf.Sqrt(count));

            for (int i = 0; i < count; i++)
            {
                int gx = i % gridWidth - gridWidth / 2;
                int gy = i / gridWidth - gridWidth / 2;
                var desired = targetCell + new Vector2Int(gx, gy);

                var unit = movers[i].Unit;
                var resolved = AdjacencyUtil.FindNearestFreeCell(simulationManager.ActiveMap, unit.Definition.Domain, simulationManager, desired);
                unit.MoveTo(simulationManager.ActiveMap, simulationManager, resolved);
            }
        }

        private void Select(UnitView view)
        {
            selected.Add(view);
            view.SetSelected(true);
        }

        private void ClearSelection()
        {
            foreach (var view in selected) view.SetSelected(false);
            selected.Clear();
        }

        private void HandleUnitDied(Unit unit)
        {
            selected.RemoveAll(view => view == null || view.Unit == unit);
        }
    }
}
