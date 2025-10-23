using Blade.Entities;
using Blade.FSM;
using Chuh007Lib.Dependencies;
using Chuh007Lib.ObjectPool.Runtime;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Work.CIW.Code;
using Work.CIW.Code.ETC;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;
using Work.CIW.Code.Player.Event;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.CUH.Code.SwitchSystem;
using Work.CUH.Code.Test;
using Work.ISC.Code.Managers;
using Work.PSB.Code.Commands;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerCode : GridObjectBase
    {
       [field: SerializeField] public PlayerInputSO InputSO { get; private set; }
        
        public override Vector3Int CurrentGridPosition { get; set; }
        
        [SerializeField] private TurnConsumeCommand turnConsumeCommand;
        
        private PSBTestPlayerMovement _movementCompo;

        [Header("FSM settings")]
        [SerializeField] StateDataSO[] states;
        EntityStateMachine _stateMachine;
        PlayerFSMHost _fsmHost;
        public EntityAnimator Animator { get; private set; }

        [Header("Turn system")]
        [field : SerializeField] public TurnSystemAdapter turnAdapter;
        [SerializeField] TurnCountManager turnManager;

        [Header("Ink Sink")]
        [SerializeField] float sinkDuration = 1f;
        [SerializeField] float sinkAmount = 0.5f;

        [Header("Object Pooling")]
        [Inject] PoolManagerMono _poolManager;
        [SerializeField] PoolingItemSO inkPool;

        [Header("Interact")] [SerializeField] private float interactRange = 0.5f;
        
        public bool IsDead = false;
        public bool IsInputLocked = false;
        private Collider[] _results = new Collider[10];
        
        public void ChangeState(string stateName, bool forced = false) => _stateMachine.ChangeState(stateName, forced);

        private void Awake()
        {
            PSBTestPlayerMovement movement = GetComponent<PSBTestPlayerMovement>();
            _movementCompo = movement;

            if (_movementCompo == null || !(_movementCompo is IMoveableTest))
            {
                Debug.LogError("PlayerMovement 컴포넌트가 IMoveableTest를 구현하지 않았습니다.");
                enabled = false;
            }

            _fsmHost = GetComponent<PlayerFSMHost>();
            if (_fsmHost == null)
            {
                Debug.LogError("PlayerFSMHost Player에 없음. 나 작동 안해");
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            _stateMachine = new EntityStateMachine(_fsmHost, states);
            Animator = GetComponentInChildren<EntityAnimator>();
            if (Animator == null)
            {
                Debug.LogWarning("EntityAnimator 못 찾겠다 꾀꼬리");
            }

            if (turnManager != null)
            {
                turnManager.OnTurnZeroEvent += HandleTurnZero;
            }
            else
            {
                Debug.LogError("TurnCountManager 연결 안됨");
            }

            Bus<GameClearEvent>.OnEvent += HandleGameClear;

            _stateMachine.ChangeState("IDLE");
        }

        private void OnEnable()
        {
            if (InputSO != null)
            {
                InputSO.OnMovement += HandleMove;
                InputSO.OnInteract += HandleInteract;
            }
        }
        
        private void OnDisable()
        {
            if (InputSO != null)
            {
                InputSO.OnMovement -= HandleMove;
                InputSO.OnInteract -= HandleInteract;
            }
        }

        private void OnDestroy()
        {
            if (turnManager != null)
            {
                turnManager.OnTurnZeroEvent -= HandleTurnZero;
            }

            Bus<GameClearEvent>.OnEvent -= HandleGameClear;
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        private void HandleInteract()
        {
            if(_movementCompo.isMoving) return;
            var size = Physics.OverlapSphereNonAlloc(transform.position, interactRange, _results);
            for (int i = 0; i < size; i++)
            {
                if (_results[i].TryGetComponent(out Lever lever))
                {
                    Bus<CommandEvent>.Raise(new CommandEvent(new SwitchCommand(lever)));
                    Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                    break;
                }
            }
        }
        
        private void HandleMove(Vector2 input)
        {
            if (IsInputLocked) return;
            if (IsDead) return;
            if (input == Vector2.zero) return;

            Vector3Int dir = GetDirection(input);
            Vector3Int curGridPos = CurrentGridPosition;
            Vector3Int frontGridPos = curGridPos + dir;
            
            Vector3 worldDirection = new Vector3(dir.x, 0, dir.z);
            if (worldDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(worldDirection, Vector3.up);
                transform.rotation = targetRotation;
            }

            Collider[] hits = Physics.OverlapSphere(frontGridPos, 0.45f);
            BlockPush blockToPush = null;
            bool isWall = false;

            foreach (Collider hit in hits)
            {
                if (hit == null) continue;
                if (hit.CompareTag("Wall"))
                {
                    isWall = true;
                    break;
                }

                if (hit.TryGetComponent(out BlockPush block))
                {
                    blockToPush = block;
                    break;
                }
            }

            if (isWall)
            {
                Bus<CommandEvent>.Raise(new CommandEvent(new NothingCommand(_movementCompo)));
                Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                Debug.Log("Wall");
                return;
            }
    
            if (blockToPush != null)
            {
                if (blockToPush.CanMove(dir))
                {
                    ChangeState("PUSH");

                    blockToPush.TryMoveByCommand(dir);
                    Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                }

                return;
            }
            if (_movementCompo.gridService != null)
            {
                if (!_movementCompo.gridService.CanMoveTo(curGridPos, dir, out _))
                {
                    Bus<CommandEvent>.Raise(new CommandEvent(new NothingCommand(_movementCompo)));
                    Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                    return;
                }
            }

            MoveCommand moveCommand = new MoveCommand(_movementCompo, input);
            
            if (moveCommand.CanExecute())
            {
                Bus<CommandEvent>.Raise(new CommandEvent(moveCommand));
                Bus<TurnUseEvent>.Raise(new TurnUseEvent());
            }

        }

        public void HandleTurnZero()
        {
            if (IsDead) return;
            if (_movementCompo.isMoving)
            {
                Debug.Log("아직 이동중이라서 죽음 처리 보류");
                return;
            }

            IsDead = true;
            _stateMachine.ChangeState("DEAD");
        }

        private void HandleGameClear(GameClearEvent evt)
        {
            if (IsDead) return;

            IsDead = true;

            IsInputLocked = true;

            _stateMachine.ChangeState("IDLE");

            Debug.Log("게임 클리어");
        }

        public IEnumerator InkPooling()
        {
            InkPool ink = _poolManager.Pop<InkPool>(inkPool);
            ink.transform.position = transform.position;

            StartCoroutine(SinkRoutine());

            yield return new WaitForSeconds(sinkDuration);

            Debug.Log("잉크 풀링 완료");
            _poolManager.Push(ink);
        }

        private IEnumerator SinkRoutine()
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos - new Vector3(0f, sinkAmount, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < sinkDuration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / sinkDuration);

                yield return null; 

                elapsedTime += Time.deltaTime;
            }

            transform.position = targetPos;
        }

        private Vector3Int GetDirection(Vector2 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return new Vector3Int(input.x > 0 ? 1 : -1, 0, 0);
            }
            else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                return new Vector3Int(0, 0, input.y > 0 ? 1 : -1);
            }

            return Vector3Int.zero;
        }

        public void SetInputLockState(bool isLocked)
        {
            IsInputLocked = isLocked;
        }

        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
            CurrentGridPosition = newPos;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
        
    }
}