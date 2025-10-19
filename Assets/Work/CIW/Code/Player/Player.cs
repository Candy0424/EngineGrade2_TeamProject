using Blade.Entities;
using Blade.FSM;
using System;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.CUH.Code.Test;
using Work.ISC.Code.Managers;

namespace Work.CIW.Code.Player
{
    public class Player : GridObjectBase
    {
        [field: SerializeField] public PlayerInputSO InputSO { get; private set; }

        [Header("FSM settings")]
        [SerializeField] StateDataSO[] states;
        EntityStateMachine _stateMachine;
        PlayerFSMHost _fsmHost;
        public EntityAnimator Animator { get; private set; }

        public event Action OnAnimationEnd;

        [Header("Manager")]
        [SerializeField] TurnCountManager turnManager;
        [SerializeField] TurnSystemAdapter turnAdapter;

        //[SerializeField] Vector3Int initialPosition = Vector3Int.zero;
        public override Vector3Int CurrentGridPosition { get; set; }
        
        IMovement _movement;

        PlayerMovement _movementCompo;

        private void Awake()
        {
            PlayerMovement movement = GetComponent<PlayerMovement>();
            _movementCompo = movement;

            // 🌟 IMoveableTest 구현 검사
            if (_movementCompo == null || !(_movementCompo is IMoveableTest))
            {
                Debug.LogError("PlayerMovement 컴포넌트가 IMoveableTest를 구현하지 않았습니다.");
                enabled = false;
            }

            _movement = GetComponent<IMovement>();

                _fsmHost = GetComponent<PlayerFSMHost>();
            if (_fsmHost == null)
            {
                Debug.LogError("PlayerFSMHost Player에 없음. 나 작동 안해");
                enabled = false;
                return;
            }

            //_stateMachine = new EntityStateMachine(_fsmHost, states);

            InputSO.OnMovement += HandleMove;
        }

        //private void OnEnable()
        //{
        //    if (InputSO != null)
        //        InputSO.OnMovement += HandleMove;
        //}

        //private void OnDisable()
        //{
        //    if (InputSO != null)
        //        InputSO.OnMovement -= HandleMove;
        //}

        private void OnDestroy()
        {
            InputSO.OnMovement -= HandleMove;

            if (turnManager != null)
            {
                turnManager.OnTurnZeroEvent -= HandleTurnZero;
            }
        }

        private void Start()
        {
            _stateMachine = new EntityStateMachine(_fsmHost, states);

            Animator = GetComponentInChildren<EntityAnimator>();

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

        private void Update()
        {
            if (!turnAdapter.HasTurnRemaining)
            {
                _stateMachine.ChangeState("DEAD");
            }

            _stateMachine.UpdateStateMachine();
        }

        public void ChangeState(string stateName, bool forecd = false) => _stateMachine.ChangeState(stateName, forecd);

        public void HandleAnimationEndEvent()
        {
            _stateMachine.CurrentState?.AnimationEndTrigger();
            OnAnimationEnd?.Invoke();
        }

        private void HandleMove(Vector2 input)
        {
            Vector2 dir = input;
            if (dir == Vector2.zero) return;

            MoveCommand command = new MoveCommand(_movementCompo, dir);


            if (command.CanExecute())
            {
                //command.Execute();
                Bus<CommandEvent>.Raise(new CommandEvent(command));
                Bus<TurnUseEvent>.Raise(new TurnUseEvent());
            }
        }

        private void HandleTurnZero()
        {
            //ChangeState("DEAD", true);
            Debug.Log("주금");
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
            // 셀에서 나갈 때 필요한 로직
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
            CurrentGridPosition = newPos;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
    }
}