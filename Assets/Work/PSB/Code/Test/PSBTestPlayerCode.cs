using Blade.Entities;
using Blade.FSM;
using System;
using UnityEngine;
using Work.CIW.Code;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
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
        [SerializeField] TurnSystemAdapter turnAdapter;
        [SerializeField] TurnCountManager turnManager;

        bool _isDead = false;

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

            _stateMachine.ChangeState("IDLE");
        }

        private void OnEnable()
        {
            if (InputSO != null)
                InputSO.OnMovement += HandleMove;
        }

        private void OnDisable()
        {
            if (InputSO != null)
                InputSO.OnMovement -= HandleMove;
        }

        private void OnDestroy()
        {
            if (!_isDead && turnAdapter != null && !turnAdapter.HasTurnRemaining)
            {
                HandleTurnZero();
            }
        }

        private void Update()
        {
            if (!turnAdapter.HasTurnRemaining)
            {
                _stateMachine.ChangeState("DEAD");
            }

            _stateMachine.UpdateStateMachine();
        }

        private void HandleMove(Vector2 input)
        {
            if (_isDead) return;

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

        private void HandleTurnZero()
        {
            Debug.Log("쥬금");
            _isDead = true;
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