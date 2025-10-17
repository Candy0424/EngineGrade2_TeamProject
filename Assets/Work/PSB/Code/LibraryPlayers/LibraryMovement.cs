using Blade.Entities;
using UnityEngine;

namespace Work.PSB.Code.LibraryPlayers
{
    public class LibraryMovement : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float rotationSpeed = 4f;
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private CharacterController controller;
        
        public bool IsGround => controller.isGrounded;
        public bool CanManualMovement { get; set; } = true;
        
        private Vector3 _autoMovement;
        private float _autoMoveStartTime;
        
        private float _moveSpeed = 8f;
        private Vector3 _velocity;
        public Vector3 Velocity => _velocity;
        private float _verticalVelocity;
        private Vector3 _movementDirection;

        private LibraryPlayer _entity;

        public void Initialize(Entity entity)
        {
            _entity = entity as LibraryPlayer;
        }

        public void AfterInitialize() { }

        public void SetMovementDirection(Vector2 movementInput)
        {
            _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        }

        private void Update()
        {
            HandleMouseRotation();
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            ApplyGravity();
            Move();
        }

        private void HandleMouseRotation()
        {
            if (!CanManualMovement || _entity.PlayerInput == null) return;
            
            Vector2 mouseDelta = _entity.PlayerInput.AimDelta;

            if (Mathf.Abs(mouseDelta.x) > 0.001f)
            {
                Vector3 euler = _entity.transform.eulerAngles;
                euler.y += mouseDelta.x * mouseSensitivity * Time.deltaTime;
                _entity.transform.eulerAngles = euler;
            }
        }

        private void CalculateMovement()
        {
            if (CanManualMovement)
            {
                _velocity = _entity.transform.rotation * _movementDirection;
                _velocity *= _moveSpeed * Time.fixedDeltaTime;
            }
            else
            {
                float normalizedTime = (Time.time - _autoMoveStartTime) / 0.5f;
                float currentSpeed = 5 * normalizedTime;
                Vector3 currentMovement = _autoMovement * currentSpeed;
                _velocity = currentMovement * Time.fixedDeltaTime;
            }
        }

        private void ApplyGravity()
        {
            if (IsGround && _verticalVelocity < 0)
                _verticalVelocity = -0.03f;
            else
                _verticalVelocity += gravity * Time.fixedDeltaTime;

            _velocity.y = _verticalVelocity;
        }

        private void Move()
        {
            controller.Move(_velocity);
        }

        public void StopImmediately()
        {
            _movementDirection = Vector3.zero;
        }
        
        
    }
}
