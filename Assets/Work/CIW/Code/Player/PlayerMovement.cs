using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Grid;

namespace Work.CIW.Code.Player
{
    #region Interfaces

    public interface IGridDataService
    {
        // Grid System에 이동 가능을 부여함
        bool CanMoveTo(Vector3Int curPos, Vector3Int dir, out Vector3Int targetPos);

        // 이동 완료 후 Grid System의 데이터를 갱신해줌
        void UpdateObjectPosition(IGridObject movingObj, Vector3Int oldPos, Vector3Int newPos);

        // Grid System이 특정 Grid Object의 위치를 초기화 할때 사용
        void SetObjectInitialPosition(IGridObject obj, Vector3Int initPos);
    }

    public interface IGridObject
    {
        Vector3Int CurrentGridPosition { get; }

        GameObject GetObject();
    }

    public interface IInteractable
    {
        bool Interact(IGridObject actor, Vector3Int dir);
    }

    public interface IMovement
    {
        void HandleInput(Vector2 input);
    }

    #endregion

    public class PlayerMovement : MonoBehaviour, IMovement, IGridObject
    {
        [Header("Dependencies - DIP")]
        [SerializeField] MonoBehaviour gridServiceMono;
        IGridDataService _gridService;

        [Header("Movement")]
        [SerializeField] float moveTime = 0.15f;

        [Header("Stair Collision Setting")]
        [SerializeField] float stairChkDistance = 0.9f;
        [SerializeField] LayerMask whatIsStair;

        public Vector3Int CurrentGridPosition { get; private set; }
        public GameObject GetGameObject() => gameObject;

        bool _isMoving = false;

        //[SerializeField] UnityEvent onMoveComplete;

        private void Awake()
        {
            if (gridServiceMono is IGridDataService service)
            {
                _gridService = service;
            }
            else
            {
                Debug.LogError("IGridDataService dependency not met. Assign GridSystem to gridServiceMono.");
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

            if (CheckForStairs(dir))
            {
                return;
            }

            if (_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int targetPos))
            {
                Debug.Log($"[PLAYER MOVEMENT] GridSystem approved move to: {targetPos}");
                StartCoroutine(MoveRoutine(targetPos));
            }
            else
            {
                Debug.LogWarning($"[PLAYER MOVEMENT] Move to {CurrentGridPosition + dir} blocked by GridSystem.");
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

            //onMoveComplete?.Invoke();
        }

        public GameObject GetObject()
        {
            return gameObject;
        }

        private bool CheckForStairs(Vector3Int dir)
        {
            Vector3 startPos = CurrentGridPosition;

            if (Physics.Raycast(startPos, dir, out RaycastHit hit, stairChkDistance))
            {
                if (hit.collider.TryGetComponent(out StairTrigger stair))
                {
                    Vector3Int targetGridPos = new Vector3Int(CurrentGridPosition.x, stair.GetTargetY(), CurrentGridPosition.z);

                    TeleportToFloor(targetGridPos);
                    return true;
                }
            }

            return false;
        }

        private void TeleportToFloor(Vector3Int targetPos)
        {
            _gridService.UpdateObjectPosition(this, CurrentGridPosition, targetPos);

            CurrentGridPosition = targetPos;
            transform.position = targetPos;

            Debug.Log($"Teleport Floor : {targetPos}");
        }
    }
}