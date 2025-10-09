using System;
using DG.Tweening;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.System;

namespace Work.PSB.Code.Test
{
    public class SpikeController : GridObjectBase
    {
        [Header("Spike Settings")]
        [SerializeField] private GameObject spikeObject;
        [SerializeField] private TurnSystem turnManager;
        [SerializeField] private float moveDistance = 3;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease easeType = Ease.OutQuad;
        [SerializeField] private bool startRaised = false;

        private bool _isRaised;
        private Vector3 _startPos;
        private Vector3 _raisedPos;
        private Tween _currentTween;
        private Collider _collider;

        private void Awake()
        {
            if (spikeObject != null)
            {
                _startPos = spikeObject.transform.localPosition;
                _raisedPos = _startPos + Vector3.up * moveDistance;
            }

            _collider = spikeObject.GetComponent<Collider>();
        }

        private void Start()
        {
            _isRaised = startRaised;
            spikeObject.transform.localPosition = _isRaised ? _raisedPos : _startPos;
            
            if (_collider != null)
                _collider.enabled = _isRaised;
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

            bool goingUp = !_isRaised;
            Vector3 targetPos = goingUp ? _raisedPos : _startPos;
            
            if (!goingUp && _collider != null)
                _collider.enabled = false;

            _isRaised = goingUp;

            _currentTween = spikeObject.transform.DOLocalMove(targetPos, moveDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    if (goingUp && _collider != null)
                        _collider.enabled = true;
                });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isRaised) return;

            PSBTestPlayerCode player = other.GetComponent<PSBTestPlayerCode>();
            if (player != null)
            {
                Bus<TurnConsumeOnlyEvent>.Raise(new TurnConsumeOnlyEvent());
                Debug.Log("플레이어 피격! 턴 1 소모");
            }
        }

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied() { }
        public override void OnCellOccupied(Vector3Int newPos) { }
        
    }
}
