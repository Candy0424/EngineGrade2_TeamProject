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

            _fsmHost.enabled = false;

            _player.ChangeState("DEAD");

            _player.StartCoroutine(DeathSequence());
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            base.Exit();
        }

        private IEnumerator DeathSequence()
        {
            yield return _player.InkPooling();

            // 끝났을 때, 책 덮고 로비로 나가야한다.
            Debug.Log("진짜로 죽음처리 끝남");
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("BookScene");
        }
    }
}