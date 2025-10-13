using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Grid;
using Work.CUH.Code.Commands;
using Work.CUH.Code.Test;

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

    public interface IMovement
    {
        void HandleInput(Vector2 input);

        bool StartMove(Vector3Int direction);
    }

    #endregion

    public class PlayerMovement : AbstractCommandable, IMovement, IMoveableTest
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

        public bool isMoving
        {
            get => _isMoving;
            set => _isMoving = value;
        }

        //public Vector3Int CurrentGridPosition { get; private set; }
        //public GameObject GetGameObject() => gameObject;

        //[SerializeField] UnityEvent onMoveComplete;

        protected override void Awake()
        {
            base.Awake();
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

        protected override void Start()
        {
            base.Start();
            Vector3Int initWorldPos = Vector3Int.RoundToInt(transform.position);

            Vector3Int initGridPos = initWorldPos;
            initGridPos.y = initGridPos.y - 1;

            _gridService.SetObjectInitialPosition(_gridObject, initGridPos);
            _gridObject.OnCellOccupied(initGridPos);
        }

        #region Player Movement

        // 입력 처리는 Player에서 해주니, 더 이상 필요 없다.
        public void HandleInput(Vector2 input)
        {
            StartMoveLogic(input);

            //if (_isMoving) return;

            //Vector3Int dir = GetDirection(input);
            //if (dir == Vector3Int.zero) return;

            //if (CheckForStairs(dir)) return;

            //if (_gridService.CanMoveTo(_gridObject.CurrentGridPosition, dir, out Vector3Int targetPos))
            //{
            //    StartCoroutine(MoveRoutine(targetPos));
            //}
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

            float startWorldY = oldPos.y + 1f;
            Vector3 start = new Vector3(oldPos.x, startWorldY, oldPos.z);

            transform.position = start;

            float targetWorldY = targetPos.y + 1f;
            Vector3 finalWorldPos = new Vector3(targetPos.x, targetWorldY, targetPos.z);

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                transform.position = Vector3.Lerp(start, finalWorldPos, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 애니메이션 종료 후 최종 위치 확정
            transform.position = finalWorldPos;

            // GridSystem에 오프셋이 없는 순수한 Grid 좌표(targetPos)를 전달
            _gridService.UpdateObjectPosition(_gridObject, oldPos, targetPos);

            _isMoving = false;
        }

        public void StartMoveLogic(Vector2 dir)
        {
            if (_isMoving) return;

            Vector3Int movementDir = GetDirection(dir);
            if (movementDir == Vector3Int.zero) return;

            StartMove(movementDir);
        }

        public bool StartMove(Vector3Int direction)
        {
            if (_isMoving) return false;

            if (CheckForStairs(direction)) return true;

            if (_gridService.CanMoveTo(_gridObject.CurrentGridPosition, direction, out Vector3Int targetPos))
            {
                StartCoroutine(MoveRoutine(targetPos));
                return true;
            }

            return false;
        }

        #endregion

        #region Check Stairs

        private bool CheckForStairs(Vector3Int dir)
        {
            //Vector3 startPos = _gridObject.CurrentGridPosition;
            //Vector3 dirVec3 = (Vector3)dir;

            //Vector3 fixedDirection = -dirVec3;

            Vector3 rayOrigin = new Vector3(_gridObject.CurrentGridPosition.x, _gridObject.CurrentGridPosition.y + 0.1f, _gridObject.CurrentGridPosition.z);
            Vector3 dirVec3 = (Vector3)dir;
            Vector3 fixedDirection = -dirVec3;

            Debug.DrawRay(rayOrigin, fixedDirection * stairChkDistance, Color.red, 1.0f);

            if (Physics.Raycast(rayOrigin, fixedDirection, out RaycastHit hit, stairChkDistance, whatIsStair))
            {
                Debug.Log("Stair 감지");

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
            Debug.Log("텔포 시킴");
        }

        #endregion
    }
}