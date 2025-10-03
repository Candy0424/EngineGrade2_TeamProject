using System;
using System.Collections;
using UnityEngine;
using Work.CIW.Code.Player;

namespace Work.PSB.Code.Test
{
    public class BlockPushTest : MonoBehaviour, IGridObject
    {
        [SerializeField] private MonoBehaviour gridServiceMono;
        
        [SerializeField] private float moveTime = 0.15f;
        [SerializeField] private bool canMoveBlock = true;

        public Vector3Int CurrentGridPosition { get; private set; }
        
        private IGridDataService _gridService;
        private bool _isMoving = false;

        private void Awake()
        {
            if (gridServiceMono is IGridDataService service)
            {
                _gridService = service;
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
            if (!canMoveBlock) return false;
            
            Vector3Int targetPos = CurrentGridPosition + dir;
            
            Collider[] hits = Physics.OverlapSphere(targetPos, 0.1f);
            foreach (Collider hit in hits)
            {
                if (hit.GetComponent<BlockPushTest>() != null)
                    return false;
                if (hit.CompareTag("Wall") || hit.CompareTag("Spike"))
                {
                    Debug.LogError("벽이나 가시가 있어 블럭을 옮길 수 없습니다.");
                    return false;
                }
            }

            return true;
        }

        public IEnumerator MoveRoutine(Vector3Int dir)
        {
            if (_isMoving) yield break;
            
            if (!_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int targetPos))
                yield break;
            if (!CanMove(dir)) yield break;

            _isMoving = true;

            Vector3Int oldPos = CurrentGridPosition;
            targetPos = oldPos + dir;

            Vector3 start = transform.position;
            Vector3 end = targetPos;

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