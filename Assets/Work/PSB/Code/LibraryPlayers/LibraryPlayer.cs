using System;
using Blade.Entities;
using Blade.FSM;
using UnityEngine;
using Work.CUH.Code.Entities;
using Work.PSB.Code.LibraryPlayer;
using Work.PSB.Code.LibraryPlayers.States;

namespace Work.PSB.Code.LibraryPlayers
{
    public class LibraryPlayer : Entity
    {
        [field: SerializeField] public LibraryPlayerInputSO PlayerInput { get; private set; }
        [SerializeField] private StateDataSO[] states;
        private EntityStateMachine _stateMachine;
        
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, states);
            PlayerInput.OnInteractionPressed += HandleInteractPressed;
        }
        
        private void OnDestroy()
        {
            PlayerInput.OnInteractionPressed -= HandleInteractPressed;
        }
        
        private void HandleInteractPressed()
        {
        }

        protected override void Start()
        {
            _stateMachine.ChangeState("IDLE");
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        public void ChangeState(string newStateName, bool forced = false) 
            => _stateMachine.ChangeState(newStateName, forced);

        
    }
}