using DG.Tweening;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.ISC.Code.System;

namespace Work.PSB.Code.Test
{
    public class SpikeControllerTest : GridObjectBase
    {
        [Header("Spike Settings")]
        [SerializeField] private GameObject spikeObject;
        [SerializeField] private TurnSystem turnManager;
        [SerializeField] private float moveDistance = 3;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease easeType = Ease.OutQuad;

        private bool _isRaised = false;
        private Vector3 _startPos;
        private Vector3 _raisedPos;
        private Tween _currentTween;

        private void Awake()
        {
            if (spikeObject != null)
            {
                _startPos = spikeObject.transform.localPosition;
                _raisedPos = _startPos + Vector3.up * moveDistance;
            }
        }

        private void OnEnable()
        {
            if (spikeObject == null) return;

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
            if (spikeObject == null) return;
            
            _currentTween?.Kill();

            _isRaised = !_isRaised;
            Vector3 targetPos = _isRaised ? _raisedPos : _startPos;

            _currentTween = spikeObject.transform.DOLocalMove(targetPos, moveDuration)
                .SetEase(easeType);
        }

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied() { }
        public override void OnCellOccupied(Vector3Int newPos) { }
    
    }
}