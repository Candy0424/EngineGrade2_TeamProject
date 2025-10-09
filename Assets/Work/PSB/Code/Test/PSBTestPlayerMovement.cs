using System.Collections;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;

namespace Work.PSB.Code.Test
{
    public class PSBTestPlayerMovement : MonoBehaviour, IMovement
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

            Vector3Int curPos = new Vector3Int(_gridObject.CurrentGridPosition.x, 
                (int)transform.position.y, _gridObject.CurrentGridPosition.z);
            
            if (_gridService.CanMoveTo(curPos, dir, out Vector3Int targetPos))
            {
                StartCoroutine(MoveRoutine(targetPos));
            }
            else
            {
                Vector3Int frontPos = curPos + dir;
                Vector3 worldFront = (Vector3)frontPos;

                Collider[] hits = Physics.OverlapSphere(worldFront, 0.45f);
                foreach (Collider hit in hits)
                {
                    if (hit == null || hit.isTrigger) continue;

                    BlockPushTest block = hit.GetComponent<BlockPushTest>();
                    if (block != null)
                    {
                        if (block.CanMove(dir))
                        {
                            StartCoroutine(block.MoveRoutine(dir));
                        }
                        return;
                    }
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

            Vector3Int oldPos = _gridObject.CurrentGridPosition;
            Vector3 start = transform.position;
            Vector3 end = (Vector3)targetPos;

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            _gridService.UpdateObjectPosition(_gridObject, oldPos, targetPos);

            _isMoving = false;
        }

        private bool CheckForStairs(Vector3Int dir)
        {
            Vector3 startPos = _gridObject.CurrentGridPosition;
            Vector3 dirVec3 = dir;
            Vector3 fixedDirection = -dirVec3;

            if (Physics.Raycast(startPos, fixedDirection, out RaycastHit hit, stairChkDistance, whatIsStair))
            {
                if (hit.collider.TryGetComponent(out StairTrigger stair))
                {
                    Vector3Int targetGridPos = new Vector3Int(
                        _gridObject.CurrentGridPosition.x,
                        stair.GetTargetY(),
                        _gridObject.CurrentGridPosition.z);

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
