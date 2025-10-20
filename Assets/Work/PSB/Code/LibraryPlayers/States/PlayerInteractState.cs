using Blade.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Work.CUH.Code.Entities;
using Work.CUH.Code.Interaction;

namespace Work.PSB.Code.LibraryPlayers.States
{
    public class PlayerInteractState : PlayerState
    {
        private PlayerInteraction _playerInteraction;
        private EntityAnimationTrigger _trigger;
        
        public PlayerInteractState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _playerInteraction = entity.GetCompo<PlayerInteraction>();
            _trigger = entity.GetCompo<EntityAnimationTrigger>();
        }

        public override void Enter()
        {
            base.Enter();
            _playerInteraction.CheckInteraction();
            _movement.StopImmediately();
            _trigger.OnAnimationEnd += HandleAnimEndTrigger;
        }
        
        public override void Update()
        {
            base.Update();
        }
        
        private void HandleAnimEndTrigger()
        {
            _player.ChangeState("IDLE");
        }
        
        public override void Exit()
        {
            base.Exit();
            _trigger.OnAnimationEnd -= HandleAnimEndTrigger;
            Debug.Log("Exiting PlayerInteractState");
        }
        
    }
}