using System.Collections;
using UnityEngine;
using Work.CIW.Code.Player;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.Test
{
    // 지금 구현상 AbstractCommandable을 굳이 상속받을 필요는 없지만, 나중에 검사할때 필요할 수도 있음.
    public class TestPlayerMovement : AbstractCommandable, IMoveableTest, IGridObject
    {
        [SerializeField] MonoBehaviour gridServiceMono;
        
        [Header("Movement")]
        [SerializeField] float moveTime = 0.15f;
        
        public Vector3Int CurrentGridPosition { get; private set; }
        public GameObject GetObject()
        {
            return gameObject;
        }
        public bool isMoving { get; set; }
        
        private IGridDataService _gridService;
        
        protected override void Awake()
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
        
        protected override void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;

            _gridService.SetObjectInitialPosition(this, CurrentGridPosition);
        }
        
        public void HandleInput(Vector2 input)
        {
            if (isMoving) return;
            
            Vector3Int dir = GetDirection(input);
            
            if (_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int targetPos))
            {
                Debug.Log($"[PLAYER MOVEMENT] GridSystem approved move to: {targetPos}");
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
            isMoving = true;

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
            
            isMoving = false;

            //onMoveComplete?.Invoke();
        }

        public void HandleInput(System.Numerics.Vector2 input)
        {
            throw new System.NotImplementedException();
        }
    }
}