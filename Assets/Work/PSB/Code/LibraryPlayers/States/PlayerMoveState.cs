using Blade.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.PSB.Code.LibraryPlayers.States
{
    public class PlayerMoveState : PlayerState
    {
        public PlayerMoveState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }
        
        public override void Update()
        {
            base.Update();
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                _player.ChangeState("INTERACT");
            }
            Vector2 movementKey = _player.PlayerInput.MovementKey;
            
            _movement.SetMovementDirection(movementKey);
            if (movementKey.magnitude < _inputThreshold)
                _player.ChangeState("IDLE");
        }
        
    }
}