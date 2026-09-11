using System.Collections.Generic;
using RTSTemplate.Simulation;
using UnityEngine;

namespace RTSTemplate.Rendering
{
    public class UnitViewRegistry : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        private readonly Dictionary<Unit, UnitView> activeViews = new Dictionary<Unit, UnitView>();

        private void OnEnable()
        {
            if (simulationManager == null) return;
            simulationManager.UnitSpawned += HandleUnitSpawned;
            simulationManager.UnitDied += HandleUnitDied;
        }

        private void OnDisable()
        {
            if (simulationManager == null) return;
            simulationManager.UnitSpawned -= HandleUnitSpawned;
            simulationManager.UnitDied -= HandleUnitDied;
        }

        public UnitView SpawnAndTrack(Unit unit)
        {
            var view = UnitView.Spawn(unit);
            if (view != null) activeViews[unit] = view;
            return view;
        }

        private void HandleUnitSpawned(Unit unit) => SpawnAndTrack(unit);

        private void HandleUnitDied(Unit unit)
        {
            if (!activeViews.TryGetValue(unit, out var view)) return;

            if (view != null) Destroy(view.gameObject);
            activeViews.Remove(unit);
        }
    }
}
