using UnityEngine;
using Work.CIW.Code;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerCode : MonoBehaviour
    {
        [field: SerializeField] public PlayerInputSO InputSO { get; private set; }

        [SerializeField] private MoveCommand moveCommand;
        
        private PSBTestPlayerMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<PSBTestPlayerMovement>();
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

        private void HandleMove(Vector2 dir)
        {
            if (_movement == null) return;
            var command = Instantiate(moveCommand);
            command.Commandable = _movement;
            command.Dir = dir;
            if (command.CanExecute())
            {
                Bus<CommandEvent>.Raise(new CommandEvent(command));
                Bus<TurnUseEvent>.Raise(new TurnUseEvent()); 
            }
        }
        
    }
}