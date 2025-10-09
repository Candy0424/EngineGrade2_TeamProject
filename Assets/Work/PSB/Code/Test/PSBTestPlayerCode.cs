using UnityEngine;
using Work.CIW.Code;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.CUH.Code.Test;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerCode : GridObjectBase
    {
       [field: SerializeField] public PlayerInputSO InputSO { get; private set; }
        public override Vector3Int CurrentGridPosition { get; set; }

        [SerializeField] MoveCommand moveCommand;

        private IMovement _movement;

        private PSBTestPlayerMovement _movementCompo;

        private void Awake()
        {
            PSBTestPlayerMovement movement = GetComponent<PSBTestPlayerMovement>();
            _movementCompo = movement;

            if (_movementCompo == null || !(_movementCompo is IMoveableTest))
            {
                Debug.LogError("PlayerMovement 컴포넌트가 IMoveableTest를 구현하지 않았습니다.");
                enabled = false;
            }

            _movement = GetComponent<IMovement>();
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

        private void HandleMove(Vector2 input)
        {
            Vector2 dir = input;
            if (dir == Vector2.zero) return;

            MoveCommand command = ScriptableObject.CreateInstance<MoveCommand>();
            command.Dir = dir;
            command.Commandable = _movementCompo;

            if (command.CanExecute())
            {
                command.Execute();
                Bus<TurnUseEvent>.Raise(new TurnUseEvent());
            }

            Destroy(command);
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
            transform.position = new Vector3(newPos.x, newPos.y + 1f, newPos.z);
        }
        
    }
}