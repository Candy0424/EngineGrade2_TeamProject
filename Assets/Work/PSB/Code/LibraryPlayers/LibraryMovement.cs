using Blade.Entities;
using Chuh007Lib.Dependencies;
using UnityEngine;

namespace Work.PSB.Code.LibraryPlayers
{
    [Provide]
    public class LibraryMovement : MonoBehaviour, IEntityComponent, IAfterInitialize, IDependencyProvider
    {
        [SerializeField] private Transform cameraPivot;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 8f;
        [SerializeField] private float gravity = -18f;
        [SerializeField] private CharacterController controller;

        [Header("Mouse Settings")]
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private float rotationSmoothTime = 0.05f;

        [Header("Head Bob Settings")]
        [SerializeField] private float walkBobFrequency = 6f;
        [SerializeField] private float walkBobAmplitude = 0.05f;
        [SerializeField] private float runBobFrequency = 9f;
        [SerializeField] private float runBobAmplitude = 0.08f;
        private Vector3 _cameraInitialLocalPos;
        private float _bobTimer = 0f;

        public bool IsGround => controller.isGrounded;
        public bool CanManualMovement { get; set; } = true;
        public float MouseSensitivity 
        { 
            get => mouseSensitivity; 
            set => mouseSensitivity = value; 
        }

        private Vector3 _velocity;
        private Vector3 _currentMoveVelocity;
        private float _verticalVelocity;
        private Vector3 _movementDirection;

        private float _cameraPitch = 0f;
        private float _currentMouseDeltaX;
        private float _mouseDeltaVelocity;
        private LibraryPlayer _entity;

        private bool _isRunning;

        public void Initialize(Entity entity)
        {
            _entity = entity as LibraryPlayer;
        }

        public void AfterInitialize()
        {
            if (cameraPivot != null)
                _cameraInitialLocalPos = cameraPivot.localPosition;
        }

        public void SetMovementDirection(Vector2 movementInput)
        {
            _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        }

        public void SetRunning(bool isRunning)
        {
            _isRunning = isRunning;
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
            HandleHeadBob();
        }

        private void HandleMouseRotation()
        {
            if (!CanManualMovement || _entity.PlayerInput == null) return;

            Vector2 mouseDelta = _entity.PlayerInput.AimDelta;

            _currentMouseDeltaX = Mathf.SmoothDamp(
                _currentMouseDeltaX,
                mouseDelta.x,
                ref _mouseDeltaVelocity,
                rotationSmoothTime
            );

            _entity.transform.Rotate(Vector3.up, _currentMouseDeltaX * MouseSensitivity * Time.deltaTime);

            _cameraPitch -= mouseDelta.y * MouseSensitivity * Time.deltaTime;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -60f, 35f);

            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
        }

        private void CalculateMovement()
        {
            if (!CanManualMovement) return;

            Vector3 targetVelocity = _entity.transform.rotation * _movementDirection * (moveSpeed * (_isRunning ? 1.6f : 1f));

            _currentMoveVelocity = Vector3.MoveTowards(
                _currentMoveVelocity,
                targetVelocity,
                (targetVelocity.magnitude > _currentMoveVelocity.magnitude ? acceleration : deceleration) * Time.fixedDeltaTime
            );

            _velocity = _currentMoveVelocity;
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
            controller.Move(_velocity * Time.fixedDeltaTime);
        }

        private void HandleHeadBob()
        {
            float idleFrequency = 1.2f;
            float idleAmplitude = 0.03f;
            
            if (!CanManualMovement || !IsGround)
            {
                cameraPivot.localPosition = Vector3.Lerp(cameraPivot.localPosition, _cameraInitialLocalPos, Time.deltaTime * 5f);
                _bobTimer = 0f;
                return;
            }

            bool isMoving = _movementDirection.magnitude >= 0.1f;
            
            float frequency = isMoving ? (_isRunning ? runBobFrequency : walkBobFrequency) : idleFrequency;
            float amplitude = isMoving ? (_isRunning ? runBobAmplitude : walkBobAmplitude) : idleAmplitude;

            _bobTimer += Time.deltaTime * frequency;

            float bobOffsetY = Mathf.Sin(_bobTimer) * amplitude;
            float bobOffsetZ = isMoving ? Mathf.Cos(_bobTimer * 0.5f) * amplitude * 0.5f : 0f;

            cameraPivot.localPosition = _cameraInitialLocalPos + new Vector3(0, bobOffsetY, bobOffsetZ);
        }

        public void StopImmediately()
        {
            _movementDirection = Vector3.zero;
            _currentMoveVelocity = Vector3.zero;
        }
        
        
    }
}
