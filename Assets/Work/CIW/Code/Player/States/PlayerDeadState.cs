using Blade.Entities;
using UnityEngine;

namespace Work.CIW.Code.Player.States
{
    public class PlayerDeadState : PlayerState
    {
        public PlayerDeadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _player.ChangeState("DEAD");
            Debug.Log("까악까악 플레이어 사망 플레이어 사망");
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
            
        }
    }
}