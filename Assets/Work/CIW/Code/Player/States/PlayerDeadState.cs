using Blade.Entities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.CIW.Code.Camera;
using Work.PSB.Code.Player;
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

            _player.IsInputLocked = true;

            //_player.InkPooling();
            _player.StartCoroutine(_player.InkPooling());
            _player.StartCoroutine(WaitTime());
        }

        public IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(2f);
            _fsmHost.OnDeadEvent?.Invoke();
            _fsmHost.enabled = false;
        }
        
    }
}