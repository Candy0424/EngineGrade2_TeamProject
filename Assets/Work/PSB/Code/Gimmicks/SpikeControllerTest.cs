using UnityEngine;
using Work.CIW.Code.Grid;
using Work.ISC.Code.System;

namespace Work.PSB.Code.Test
{
    public class SpikeControllerTest : GridObjectBase
    {
        [SerializeField] private GameObject spikeObject;
        [SerializeField] private TurnManager turnManager;
        private bool _isActive;

        private void OnEnable()
        {
            if (spikeObject == null) return;

            _isActive = spikeObject.activeSelf;

            if (turnManager != null)
                turnManager.OnUseTurn += ToggleSpike;
        }

        private void OnDisable()
        {
            if (turnManager != null)
                turnManager.OnUseTurn -= ToggleSpike;
        }

        private void ToggleSpike()
        {
            _isActive = !_isActive;
            spikeObject.SetActive(_isActive);
        }

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied() { }
        public override void OnCellOccupied(Vector3Int newPos) { }
    }
}