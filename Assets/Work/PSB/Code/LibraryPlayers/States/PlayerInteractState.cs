using Blade.Entities;
using UnityEngine;

namespace Work.PSB.Code.LibraryPlayers.States
{
    public class PlayerInteractState : PlayerState
    {
        public PlayerInteractState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entering PlayerInteractState");
        }
        
        public override void Update()
        {
            base.Update();
            Vector2 movementKey = _player.PlayerInput.MovementKey;
            
            _movement.SetMovementDirection(movementKey);
            if (movementKey.magnitude > _inputThreshold)
                _player.ChangeState("MOVE");
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exiting PlayerInteractState");
        }
        
    }
}