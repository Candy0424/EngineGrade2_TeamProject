using Blade.Entities;
using System.Collections;
using UnityEngine;
using Work.PSB.Code.Player;

namespace Work.CIW.Code.Player.States
{
    public class PlayerPushState : PlayerState
    {
        public PlayerPushState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity.GetComponent<PSBTestPlayerCode>();
        }

        public override void Enter()
        {
            base.Enter();

            _player.StartCoroutine(WaitTime());
        }

        private IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(0.5f);
            _player.ChangeState("IDLE");
        }
    }
}