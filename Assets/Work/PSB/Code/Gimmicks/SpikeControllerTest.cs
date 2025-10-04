using UnityEngine;
using Work.ISC.Code.System;

namespace Work.PSB.Code.Test
{
    public class SpikeControllerTest : MonoBehaviour
    {
        [SerializeField] private GameObject spikeObject;
        [SerializeField] private TurnManager turnManager;
        private bool _isActive;

        private void OnEnable()
        {
            _isActive = spikeObject.activeSelf;
            if (turnManager != null)
                turnManager.OnUseTurn += ToggleSpike;
        }

        private void OnDestroy()
        {
            if (turnManager != null)
                turnManager.OnUseTurn -= ToggleSpike;
        }

        private void ToggleSpike()
        {
            _isActive = !_isActive;
            spikeObject.SetActive(_isActive);
        }
        
    }
}