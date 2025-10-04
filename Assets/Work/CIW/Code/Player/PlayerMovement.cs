using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Grid;

namespace Work.CIW.Code.Player
{
    #region Interfaces

    public interface IGridDataService
    {
        // Grid System???대룞 媛?μ쓣 遺?ы븿
        bool CanMoveTo(Vector3Int curPos, Vector3Int dir, out Vector3Int targetPos);

        // ?대룞 ?꾨즺 ??Grid System???곗씠?곕? 媛깆떊?댁쨲
        void UpdateObjectPosition(GridObjectBase movingObj, Vector3Int oldPos, Vector3Int newPos);

        // Grid System???뱀젙 Grid Object???꾩튂瑜?珥덇린???좊븣 ?ъ슜
        void SetObjectInitialPosition(GridObjectBase obj, Vector3Int initPos);
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

    public class PlayerMovement : MonoBehaviour, IMovement
    {
        [Header("Dependencies - DIP")]
        [SerializeField] MonoBehaviour gridServiceMono;
        IGridDataService _gridService;
        GridObjectBase _gridObject;

        bool _isMoving = false;

        [Header("Movement")]
        [SerializeField] float moveTime = 0.15f;

        [Header("Stair Collision Setting")]
        [SerializeField] float stairChkDistance = 1.01f;
        [SerializeField] LayerMask whatIsStair;

        //public Vector3Int CurrentGridPosition { get; private set; }
        //public GameObject GetGameObject() => gameObject;

        //[SerializeField] UnityEvent onMoveComplete;

        private void Awake()
        {
            if (gridServiceMono is IGridDataService service)
            {
                _gridService = service;
            }
            else
            {
                Debug.LogError("IGridDataService dependency not met. Assign GridSystem component to 'gridServiceMono'.");
                enabled = false;
            }

            _gridObject = GetComponent<GridObjectBase>();
            if (_gridObject == null)
            {
                Debug.LogError("Player object must inherit from GridObjectBase.");
                enabled = false;
            }
        }

        public void HandleInput(Vector2 input)
        {
            if (_isMoving) return;

            Vector3Int dir = GetDirection(input);
            if (dir == Vector3Int.zero) return;

            if (CheckForStairs(dir)) return;

            if (_gridService.CanMoveTo(_gridObject.CurrentGridPosition, dir, out Vector3Int targetPos))
            {
                StartCoroutine(MoveRoutine(targetPos));
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
            Vector3Int oldPos = _gridObject.CurrentGridPosition;

            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < moveTime) 
            {
                transform.position = Vector3.Lerp(start, targetPos, elapsed / moveTime);
                elapsed += Time.deltaTime;

                yield return null;
            }

            transform.position = targetPos;

            _gridService.UpdateObjectPosition(_gridObject, oldPos, targetPos);

            _isMoving = false;

            yield break;
        }

        private bool CheckForStairs(Vector3Int dir)
        {
            Vector3 startPos = _gridObject.CurrentGridPosition;
            Vector3 dirVec3 = (Vector3)dir;

            Vector3 fixedDirection = -dirVec3;

            // ... (Debug.DrawRay 및 Raycast 로직 유지) ...

            if (Physics.Raycast(startPos, fixedDirection, out RaycastHit hit, stairChkDistance, whatIsStair))
            {
                if (hit.collider.TryGetComponent(out StairTrigger stair))
                {
                    Vector3Int targetGridPos = new Vector3Int(_gridObject.CurrentGridPosition.x, stair.GetTargetY(), _gridObject.CurrentGridPosition.z);

                    TeleportToFloor(targetGridPos);
                    return true;
                }
            }
            return false;
        }

        private void TeleportToFloor(Vector3Int targetPos)
        {
            _gridService.UpdateObjectPosition(_gridObject, _gridObject.CurrentGridPosition, targetPos);
        }
    }
}