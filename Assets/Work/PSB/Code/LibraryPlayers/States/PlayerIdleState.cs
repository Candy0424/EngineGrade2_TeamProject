using Blade.Entities;
using UnityEngine;

namespace Work.PSB.Code.LibraryPlayers.States
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();
            Vector2 movementKey = _player.PlayerInput.MovementKey;
            
            _movement.SetMovementDirection(movementKey);
            if (movementKey.magnitude > _inputThreshold)
                _player.ChangeState("MOVE");
        }
        
    }
}