using UnityEngine;
using Work.CIW.Code;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.CUH.Code.Test;
using Work.PSB.Code.Commands;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerCode : GridObjectBase
    {
       [field: SerializeField] public PlayerInputSO InputSO { get; private set; }
        public override Vector3Int CurrentGridPosition { get; set; }
        
        [SerializeField] private MoveCommand moveCommand;
        [SerializeField] private TurnConsumeCommandSO turnConsumeCommand;
        
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
            if (input == Vector2.zero) return;

            Vector3Int dir = GetDirection(input);
            Vector3Int curGridPos = CurrentGridPosition;
            Vector3Int frontGridPos = curGridPos + dir;

            Collider[] hits = Physics.OverlapSphere(frontGridPos, 0.45f);
            BlockPush blockToPush = null;
            bool isWall = false;
            bool isSpike = false;

            foreach (Collider hit in hits)
            {
                if (hit == null) continue;

                if (hit.CompareTag("Wall"))
                {
                    isWall = true;
                    break;
                }
                
                foreach (Transform child in hit.transform)
                {
                    if (child.CompareTag("Spike"))
                    {
                        Collider childCollider = child.GetComponent<Collider>();
                        if (childCollider != null && !childCollider.enabled)
                        {
                            Debug.Log($"Spike 감지됨: {child.name}");
                            isSpike = true;
                            break;
                        }
                    }
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
                return;
            }

            if (isSpike)
            {
                Debug.Log("isSpike");
                TurnConsumeCommandSO turnCmd = Instantiate(turnConsumeCommand);
                Bus<CommandEvent>.Raise(new CommandEvent(turnCmd));
            }
    
            if (blockToPush != null)
            {
                if (blockToPush.CanMove(dir))
                {
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
            
            MoveCommand moveCommand = ScriptableObject.CreateInstance<MoveCommand>();
            moveCommand.Dir = input;
            moveCommand.Commandable = _movementCompo;

            if (moveCommand.CanExecute())
            {
                Bus<CommandEvent>.Raise(new CommandEvent(moveCommand));
                Bus<TurnUseEvent>.Raise(new TurnUseEvent());
            }
            Destroy(moveCommand);
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