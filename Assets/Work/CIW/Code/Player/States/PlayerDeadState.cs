using Blade.Entities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.CIW.Code.Camera;
using Work.PSB.Code.Test;

namespace Work.CIW.Code.Player.States
{
    public class PlayerDeadState : PlayerState
    {
        readonly PlayerFSMHost _fsmHost;

        public PlayerDeadState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity.GetComponent<PSBTestPlayerCode>();
            _fsmHost = entity.GetComponent<PlayerFSMHost>();
        }

        public override void Enter()
        {
            base.Enter();

            _player.StopAllCoroutines();

            _player.InkPooling();
            _fsmHost.OnDeadEvent?.Invoke();
            _fsmHost.enabled = false;
        }

        
    }
}