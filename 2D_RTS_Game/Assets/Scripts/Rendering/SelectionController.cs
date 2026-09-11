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
        [SerializeField] private float clickTolerance = 0.5f;

        private readonly List<UnitView> selected = new List<UnitView>();
        private Vector2 dragStartScreen;
        private bool dragging;
        private bool hasDragStart;

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
                IssueMoveOrder(mouse.position.ReadValue());
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
                var screenPos = cam.WorldToScreenPoint(view.transform.position);
                if (screenPos.x >= min.x && screenPos.x <= max.x && screenPos.y >= min.y && screenPos.y <= max.y)
                    Select(view);
            }
        }

        private void SelectSingleAt(Vector2 screenPos)
        {
            ClearSelection();

            var world = cam.ScreenToWorldPoint(screenPos);
            UnitView closest = null;
            float closestDist = clickTolerance;

            foreach (var view in FindObjectsByType<UnitView>(FindObjectsSortMode.None))
            {
                float dist = Vector2.Distance(world, view.transform.position);
                if (dist < closestDist)
                {
                    closest = view;
                    closestDist = dist;
                }
            }

            if (closest != null) Select(closest);
        }

        private void IssueMoveOrder(Vector2 screenPos)
        {
            if (simulationManager.ActiveMap == null) return;

            var worldTarget = cam.ScreenToWorldPoint(screenPos);
            var targetCell = new Vector2Int(Mathf.FloorToInt(worldTarget.x), Mathf.FloorToInt(worldTarget.y));

            var centroid = Vector2.zero;
            foreach (var view in selected) centroid += (Vector2)view.transform.position;
            centroid /= selected.Count;

            foreach (var view in selected)
            {
                var offset = (Vector2)view.transform.position - centroid;
                var unitTarget = targetCell + new Vector2Int(Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y));
                view.Unit.MoveTo(simulationManager.ActiveMap, unitTarget);
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
    }
}
