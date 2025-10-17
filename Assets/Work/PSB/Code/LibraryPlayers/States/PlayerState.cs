using Blade.Entities;
using Blade.FSM;
using UnityEngine;

namespace Work.PSB.Code.LibraryPlayers.States
{
    public class PlayerState : EntityState
    {
        protected LibraryPlayer _player;
        protected readonly float _inputThreshold = 0.1f;

        protected LibraryMovement _movement;
        public PlayerState(Entity entity, int animationHash) : base(entity, animationHash)
        {
            _player = entity as LibraryPlayer;
            _movement = entity.GetCompo<LibraryMovement>();
        }
        
    }
}