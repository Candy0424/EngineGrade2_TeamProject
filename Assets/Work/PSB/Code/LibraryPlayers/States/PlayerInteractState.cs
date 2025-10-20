using Blade.Entities;
using UnityEngine;
using Work.CUH.Code.Interaction;

namespace Work.PSB.Code.LibraryPlayers.States
{
    public class PlayerInteractState : PlayerState
    {
        private PlayerInteraction _playerInteraction;
        
        public PlayerInteractState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _playerInteraction = entity.GetCompo<PlayerInteraction>();
        }

        public override void Enter()
        {
            base.Enter();
            _playerInteraction.CheckInteraction();
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