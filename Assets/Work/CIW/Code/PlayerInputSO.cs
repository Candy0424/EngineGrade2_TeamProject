using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.CIW.Code
{
    [CreateAssetMenu(fileName = "InputSO", menuName = "SO/Input")]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public Vector3 MoveInput { get; private set; }

        public event Action<Vector2> OnMovement;
        public event Action OnInteract;

        Controls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }

            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MoveInput = context.ReadValue<Vector2>();
                OnMovement?.Invoke(MoveInput);
            }
            if (context.canceled)
                MoveInput = Vector2.zero;
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteract?.Invoke();
        }
    }
}