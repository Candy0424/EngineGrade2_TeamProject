using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.PSB.Code.LibraryPlayer
{
    [CreateAssetMenu(fileName = "LibraryPlayerInput", menuName = "SO/LibraryPlayerInput", order = 0)]
    public class LibraryPlayerInputSO : ScriptableObject, LibraryControls.IPlayersActions
    {
        [SerializeField] private LayerMask whatIsGround;
        public event Action OnInteractionPressed;
        
        public Vector2 MovementKey { get; private set; }
        public Vector2 AimDelta { get; private set; }
        
        private LibraryControls _controls;
        private Vector2 _screenPosition;
        private Vector3 _worldPosition; 
        
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new LibraryControls();
                _controls.Players.SetCallbacks(this);
            }
            _controls.Players.Enable();
        }
    
        private void OnDisable()
        {
            _controls.Players.Disable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MovementKey = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            AimDelta = context.ReadValue<Vector2>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteractionPressed?.Invoke();
        }
        
        public Vector3 GetWorldPosition()
        {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "No main camera found.");
            Ray camRay = mainCam.ScreenPointToRay(_screenPosition);
            if (Physics.Raycast(camRay, out RaycastHit hit, mainCam.farClipPlane, whatIsGround))
            {
                _worldPosition = hit.point;
            }
            return _worldPosition;
        }
        
    }
}