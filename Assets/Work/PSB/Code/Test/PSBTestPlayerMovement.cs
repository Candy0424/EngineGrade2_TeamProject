using System.Collections;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;
using Work.CUH.Code.Commands;
using Work.CUH.Code.Test;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerMovement : AbstractCommandable, IMovement, IMoveableTest
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
        
        protected override void Awake()
        {
            base.Awake();
            if (gridServiceMono is IGridDataService service)
            {
                _gridService = service;
            }
            else
            {
                enabled = false;
            }
            
            _gridObject = GetComponent<GridObjectBase>();
            if (_gridObject == null)
            {
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
        
        public void HandleInput(Vector2 input)
        {
            if (_isMoving) return;

            Vector3Int dir = GetDirection(input);
            if (dir == Vector3Int.zero) return;

            if (CheckForStairs(dir)) return;

            Vector3Int curPos = _gridObject.CurrentGridPosition;

            #region fixed

            Vector3Int frontPos = curPos + dir; // added
            Vector3 worldFront = (Vector3)frontPos; // added

            Collider[] hits = Physics.OverlapSphere(worldFront, 0.45f); // added

            BlockPush blockToPush = null;
            bool isWallOrSpike = false;

            foreach (Collider hit in hits)
            {
                if (hit == null) continue;

                if (hit.CompareTag("Wall") || hit.CompareTag("Spike"))
                {
                    isWallOrSpike = true;
                    break;
                }

                BlockPush block = hit.GetComponent<BlockPush>();
                if (block != null)
                {
                    blockToPush = block;
                    break;
                }
            }

            if (isWallOrSpike)
            {
                Debug.Log("Wall/Spike가 감지되어 이동을 취소합니다.");
                return;
            }

            if (blockToPush != null)
            {
                Debug.Log("Block 감지됨. 밀기 시도.");

                if (blockToPush.CanMove(dir))
                {
                    StartCoroutine(blockToPush.MoveRoutine(dir));
                    Debug.Log("블록 밀기 시작");
                }
                else
                {
                    Debug.Log("블록 밀기 실패 (뒤에 장애물).");
                }

                return;
            }

            if (_gridService.CanMoveTo(curPos, dir, out Vector3Int targetPos))
            {
                StartMoveLogic(input);
            }

            #endregion

            //Vector3Int curPos = new Vector3Int(
            //    _gridObject.CurrentGridPosition.x,
            //    (int)transform.position.y,
            //    _gridObject.CurrentGridPosition.z
            //);

            //if (_gridService.CanMoveTo(curPos, dir, out Vector3Int targetPos))
            //{
            //    StartMoveLogic(input);
            //}
            //else
            //{
            //    Vector3Int frontPos = curPos + dir;
            //    Vector3 worldFront = (Vector3)frontPos;

            //    Collider[] hits = Physics.OverlapSphere(worldFront, 0.45f);
            //    foreach (Collider hit in hits)
            //    {
            //        if (hit == null || hit.isTrigger) continue;

            //        BlockPush block = hit.GetComponent<BlockPush>();
            //        if (block != null)
            //        {
            //            if (block.CanMove(dir))
            //            {
            //                StartCoroutine(block.MoveRoutine(dir));
            //            }
            //            return;
            //        }
            //    }
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
            
            transform.position = finalWorldPos;

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
            Vector3 startPos = _gridObject.CurrentGridPosition;
            Vector3 dirVec3 = (Vector3)dir;

            Vector3 fixedDirection = -dirVec3;

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

        #endregion
        
    }
}
