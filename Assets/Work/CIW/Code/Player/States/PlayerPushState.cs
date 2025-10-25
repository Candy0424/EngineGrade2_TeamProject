using Blade.Entities;
using System.Collections;
using UnityEngine;
using Work.PSB.Code.Test;

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
            Debug.Log("기다려");
            yield return new WaitForSeconds(2.6f);
            _player.ChangeState("IDLE");
        }

        //public override void Update()
        //{
            
        //}

        //public override void Exit()
        //{
        //    // 여기서 해주면 에디터 터진다ㅏㅏㅏㅏ
        //    // 그러면 어디서 해줄건데
        //    // 모르겠어. 어디서 해줘야하지
        //    // 애초에 여기는 push 끝나고 한 칸 이동하고 들어오잖아.
        //    //_player.ChangeState("IDLE", true);

        //    base.Exit();
        //}
    }
}