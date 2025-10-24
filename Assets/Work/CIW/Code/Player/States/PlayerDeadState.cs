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

            Debug.Log("Dead 들어옴");

            _player.StopAllCoroutines();

            _player.IsInputLocked = true;
            Debug.Log($"바꿔줌. IsInputLocked : {_player.IsInputLocked}");

            //_player.InkPooling();
            _player.StartCoroutine(_player.InkPooling());
            _player.StartCoroutine(WaitTime());
        }

        public IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(2f);
            _fsmHost.OnDeadEvent?.Invoke();
            Debug.Log("DeadEvent호출");
            _fsmHost.enabled = false;
        }
        
    }
}