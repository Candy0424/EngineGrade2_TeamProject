using System;
using System.Collections;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;

namespace Work.PSB.Code.Test
{
    public class BlockPush : GridObjectBase
    {
        [SerializeField] private MonoBehaviour gridServiceMono;
        [SerializeField] private float moveTime = 0.15f;
        [SerializeField] private bool canMoveBlock = true;

        public override Vector3Int CurrentGridPosition { get; set; }

        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
        }

        private IGridDataService _gridService;
        private bool _isMoving = false;

        private void Awake()
        {
            if (gridServiceMono is IGridDataService service)
            {
                _gridService = service;
            }
            else
            {
                Debug.LogError("GridService가 올바르게 할당되지 않았습니다!");
            }
        }

        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;

            _gridService.SetObjectInitialPosition(this, CurrentGridPosition);
        }

        public bool CanMove(Vector3Int dir)
        {
            #region fixed

            if (!canMoveBlock)
            {
                Debug.Log("canMoveBlock is false");
                return false;
            }

            Vector3Int targetPos = CurrentGridPosition + dir;
            
            Collider[] hits = Physics.OverlapSphere((Vector3)targetPos, 0.45f);
            foreach (Collider hit in hits)
            {
                if (hit == null) continue;

                if (hit.GetComponent<BlockPush>() != null)
                {
                    Debug.LogError("CanMove: 블록이 밀릴 칸에 다른 BlockPush 오브젝트가 있습니다.");
                    return false;
                }
                if (hit.GetComponent<SpikeController>() != null)
                {
                    Debug.LogError("CanMove: 블록이 밀릴 칸에 다른 Spike 오브젝트가 있습니다.");
                    return false;
                }
                if (hit.CompareTag("Wall") || hit.CompareTag("Spike"))
                {
                    Debug.LogError("CanMove: 블록이 밀릴 칸에 Wall/Spike가 있습니다.");
                    return false;
                }
            }

            Debug.Log("CanMove: 블록을 밀 수 있습니다. MoveRoutine 시작!");
            return true;

            #endregion
        }

        public IEnumerator MoveRoutine(Vector3Int dir)
        {
            if (_isMoving) yield break;

            Vector3Int oldPos = CurrentGridPosition;
            Vector3Int targetPos = oldPos + dir;

            /*if (!_gridService.CanMoveTo(oldPos, dir, out _))
                yield break;*/
            
            if (!CanMove(dir)) yield break;

            _isMoving = true;

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
            CurrentGridPosition = targetPos;

            _gridService.UpdateObjectPosition(this, oldPos, targetPos);

            _isMoving = false;
        }

        public GameObject GetObject()
        {
            return gameObject;
        }
        
        
    }
}
