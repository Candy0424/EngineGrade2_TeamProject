using Blade.Entities;
using Blade.FSM;
using UnityEngine;
using Work.PSB.Code.Test;

namespace Work.CIW.Code.Player.States
{
    public class PlayerState : EntityState
    {
        //protected Player _player;
        //protected PlayerMovement _movement;

        protected PSBTestPlayerCode _player;
        protected PSBTestPlayerMovement _movement;
        protected readonly float _inputThreshold = 0.1f;

        public PlayerState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            GameObject player = entity.gameObject;

            //_player = player.GetComponent<Player>();
            //_movement = player.GetComponent<PlayerMovement>();
            _player = player.GetComponent<PSBTestPlayerCode>();
            _movement = player.GetComponent<PSBTestPlayerMovement>();

            Debug.Assert(_player != null, $"[PlayerState] Player 컴포넌트를 찾을 수 없습니다. (GameObject: {player.name})");
            Debug.Assert(_movement != null, $"[PlayerState] PlayerMovement 컴포넌트를 찾을 수 없습니다. (GameObject: {player.name})");
        }

        public override void AnimationEndTrigger()
        {
            base.AnimationEndTrigger();
        }
    }
}