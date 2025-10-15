using System;
using Chuh007Lib.Dependencies;
using Chuh007Lib.ObjectPool.Runtime;
using DG.Tweening;
using UnityEngine;
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
        [SerializeField] private float moveDistance = 3;
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private Ease easeType = Ease.OutQuad;
        [SerializeField] private bool startRaised = false;
        
        [Header("Command / Effect")]
        [SerializeField] private SpikeCommandSO spikeCommand;
        [SerializeField] private TurnConsumeCommandSO turnConsumeCommand;
        [SerializeField] private PoolingItemSO bloodEffect;

        [Inject] private PoolManagerMono _poolManager;

        private bool _isRaised;
        private bool _hasHitPlayerThisCycle = false;
        private bool _isFirst = true;
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
                //_raisedPos = _startPos + Vector3.back * moveDistance;
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
                turnManager.OnUseTurn += OnTurnUse;
        }

        private void OnDisable()
        {
            if (turnManager != null)
                turnManager.OnUseTurn -= OnTurnUse;
        }

        private void OnTurnUse()
        {
            SpikeCommandSO command = Instantiate(spikeCommand);
            command.Commandable = this;
            Bus<CommandEvent>.Raise(new CommandEvent(command));

            if (_isFirst)
            {
                command.Execute();    
            }
            _isFirst = false;
            Debug.Log("Spike CommandEvent Use");
        }
        
        public void ToggleSpikeCommanded()
        {
            if (spikeObject == null) return;

            _currentTween?.Kill();

            bool goingUp = !_isRaised;
            Vector3 targetPos = goingUp ? _raisedPos : _startPos;

            if (!goingUp && _collider != null)
                _collider.enabled = false;

            _isRaised = goingUp;
            
            if (goingUp)
                _hasHitPlayerThisCycle = false;

            _currentTween = spikeObject.transform.DOLocalMove(targetPos, moveDuration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    if (goingUp && _collider != null)
                        _collider.enabled = true;
                    Debug.Log("Spike Toggle Complete");
                });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isRaised || _hasHitPlayerThisCycle) return;

            PSBTestPlayerCode player = other.GetComponent<PSBTestPlayerCode>();
            if (player != null)
            {
                _hasHitPlayerThisCycle = true;
                
                TurnConsumeCommandSO turnCmd = Instantiate(turnConsumeCommand);
                Bus<CommandEvent>.Raise(new CommandEvent(turnCmd));
                Debug.Log("Spike Trigger CommandEvent Use");
                
                CreateEffect();
                Debug.Log("플레이어 피격! 턴 1 소모");
            }
        }
        
        public async void CreateEffect()
        {
            PoolingEffect effect = _poolManager.Pop<PoolingEffect>(bloodEffect);
            effect.PlayVFX(transform.position);
            await Awaitable.WaitForSecondsAsync(2f);
            _poolManager.Push(effect);
        }

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied() { }
        public override void OnCellOccupied(Vector3Int newPos) { }
        
    }
}
