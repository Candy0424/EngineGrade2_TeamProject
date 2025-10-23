using System;
using Chuh007Lib.Dependencies;
using Chuh007Lib.ObjectPool.Runtime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.Effects;
using Work.ISC.Code.System;
using Work.PSB.Code.Commands;

namespace Work.PSB.Code.Test
{
    public class SpikeController : GridObjectBase, ICommandable
    {
        [Header("Spike Settings")]
        [SerializeField] private GameObject spikeObject;
        [SerializeField] private TurnSystem turnManager;
        [SerializeField] private bool startRaised = false;
        [SerializeField] private bool isWork = true;
        
        [Header("DoTween Settings")]
        [SerializeField] private float moveDistance = 3;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease easeType = Ease.OutQuad;
        
        [Header("Command / Effect")]
        [SerializeField] private PoolingItemSO bloodEffect;

        [Header("Event")] 
        public UnityEvent OnPlayerHit;

        [Inject] private PoolManagerMono _poolManager;

        private bool _isRaised;
        private Vector3 _startPos;
        private Vector3 _raisedPos;
        private Tween _currentTween;
        private Collider _collider;

        public bool IsRaised => _isRaised;

        private void Awake()
        {
            if (spikeObject != null)
            {
                _startPos = spikeObject.transform.localPosition;
                _raisedPos = _startPos + Vector3.back * moveDistance;
            }

            _collider = spikeObject.GetComponent<Collider>();
            Bus<PlayerPosChangeEvent>.OnEvent += HandlePlayerPosChange;
        }

        public void StartEvent()
        {
            _isRaised = startRaised;
            spikeObject.transform.localPosition = _isRaised ? _startPos : _raisedPos;
            
            if (_collider != null)
                _collider.enabled = _isRaised;
            SpikeCommand command = new SpikeCommand(this);
            Bus<CommandEvent>.Raise(new CommandEvent(command));
        }

        private void OnEnable()
        {
            if (spikeObject == null) return;
            if (turnManager != null && isWork)
                turnManager.OnUseTurn += OnTurnUse;
        }

        private void OnDisable()
        {
            if (turnManager != null && isWork)
                turnManager.OnUseTurn -= OnTurnUse;
        }
        
        private void OnDestroy()
        {
            Bus<PlayerPosChangeEvent>.OnEvent -= HandlePlayerPosChange;
        }
        
        private void OnTurnUse()
        {
            SpikeCommand command = new SpikeCommand(this);
            Bus<CommandEvent>.Raise(new CommandEvent(command));

        }
        
        public void ToggleSpikeCommanded()
        {
            if (spikeObject == null) return;

            _currentTween?.Kill();

            bool goingUp = !_isRaised;
            Vector3 targetPos = goingUp ? _startPos : _raisedPos;

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
        
        public async void CreateEffect()
        {
            PoolingEffect effect = _poolManager.Pop<PoolingEffect>(bloodEffect);
            effect.PlayVFX(transform.position);
            await Awaitable.WaitForSecondsAsync(2f);
            _poolManager.Push(effect);
        }
        
        private void HandlePlayerPosChange(PlayerPosChangeEvent evt)
        {
            if (!_isRaised) return;
            
            if (Vector3.Distance(evt.transform.position + evt.direction, transform.position) <= 0.05f)
            {
                Bus<CommandEvent>.Raise(new CommandEvent(new TurnConsumeCommand()));
                CreateEffect();
                OnPlayerHit?.Invoke();
            }
        }
        
        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied() { }
        public override void OnCellOccupied(Vector3Int newPos) { }
        
    }
}
