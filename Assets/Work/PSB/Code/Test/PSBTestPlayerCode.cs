using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Player;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerCode : MonoBehaviour, IMovement, IGridObject
    {
        [Header("Dependencies - DIP")]
        [SerializeField] MonoBehaviour gridServiceMono;
        IGridDataService _gridService;

        [Header("Movement")]
        [SerializeField] float moveTime = 0.15f;

        public Vector3Int CurrentGridPosition { get; private set; }
        public GameObject GetGameObject() => gameObject;

        bool _isMoving = false;

        [SerializeField] public UnityEvent OnActionComplete;

        private void Awake()
        {
            if (gridServiceMono is IGridDataService service)
            {
                _gridService = service;
            }
            else
            {
                enabled = false;
            }
        }

        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;

            _gridService.SetObjectInitialPosition(this, CurrentGridPosition);
        }

        public void HandleInput(Vector2 input)
        {
            if (_isMoving) return;

            Vector3Int dir = GetDirection(input);
            if (dir == Vector3Int.zero) return;

            Vector3Int nextPos = CurrentGridPosition + dir;
            
            Collider[] hits = Physics.OverlapSphere(nextPos, 0.1f);
            bool blockFound = false;

            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out BlockPushTest block)) 
                {
                    blockFound = true;

                    if (block.CanMove(dir))
                    {
                        StartCoroutine(block.MoveRoutine(dir));
                        OnActionComplete?.Invoke();
                    }
                    return;
                }
            }
 
            if (!blockFound)
            {
                if (_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int targetPos))
                {
                    StartCoroutine(MoveRoutine(targetPos));
                }
            }
        }

        private Vector3Int GetDirection(Vector2 input)
        {
            if (input.y > 0.5f) return Vector3Int.forward;
            if (input.y < -0.5f) return Vector3Int.back;

            if (input.x > 0.5f) return Vector3Int.right;
            if (input.x < -0.5f) return Vector3Int.left;
            
            return Vector3Int.zero;
        }

        private IEnumerator MoveRoutine(Vector3Int targetPos)
        {
            _isMoving = true;

            Vector3 start = transform.position;

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                transform.position = Vector3.Lerp(start, targetPos, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            _gridService.UpdateObjectPosition(this, CurrentGridPosition, targetPos);
            CurrentGridPosition = targetPos;

            _isMoving = false;

            OnActionComplete?.Invoke();
        }

        public GameObject GetObject()
        {
            return gameObject;
        }
        
    }
}