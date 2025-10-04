//using System.Collections;
//using UnityEngine;
//using Work.CIW.Code.Player;
//using Work.CUH.Code.Commands;
//using Work.CUH.Code.Test;

//namespace Work.PSB.Code.Test
//{
//    public class PSBTestPlayerMovement : AbstractCommandable, IMoveableTest, IGridObject
//    {
//        [SerializeField] MonoBehaviour gridManager;
        
//        [Header("Movement")]
//        [SerializeField] float moveTime = 0.15f;
        
//        public Vector3Int CurrentGridPosition { get; private set; }
//        public bool isMoving { get; set; }

//        private IGridDataService _gridService;
        
//        public GameObject GetObject() => gameObject;

//        protected override void Awake()
//        {
//            if (gridManager is IGridDataService service)
//            {
//                _gridService = service;
//            }
//            else
//            {
//                Debug.LogError("IGridDataService dependency not met. Assign GridSystem to gridServiceMono.");
//                enabled = false;
//            }
//        }
        
//        protected override void Start()
//        {
//            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
//            transform.position = CurrentGridPosition;

//            _gridService.SetObjectInitialPosition(this, CurrentGridPosition);
//        }

//        public void HandleInput(Vector2 input)
//        {
//            if (isMoving) return;

//            Vector3Int dir = GetDirection(input);
//            if (dir == Vector3Int.zero) return;

//            Vector3Int nextPos = CurrentGridPosition + dir;
            
//            Collider[] hits = Physics.OverlapSphere(nextPos, 0.1f);
//            bool blockFound = false;

//            foreach (Collider hit in hits)
//            {
//                if (hit.TryGetComponent(out BlockPushTest block)) 
//                {
//                    blockFound = true;

//                    if (block.CanMove(dir))
//                    {
//                        StartCoroutine(block.MoveRoutine(dir));
//                    }
//                    return;
//                }
//            }

//            if (!blockFound)
//            {
//                if (_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int targetPos))
//                {
//                    StartCoroutine(MoveRoutine(targetPos));
//                }
//            }
//        }

//        private Vector3Int GetDirection(Vector2 input)
//        {
//            if (input.y > 0.5f) return Vector3Int.forward;
//            if (input.y < -0.5f) return Vector3Int.back;

//            if (input.x > 0.5f) return Vector3Int.right;
//            if (input.x < -0.5f) return Vector3Int.left;
            
//            return Vector3Int.zero;
//        }

//        private IEnumerator MoveRoutine(Vector3Int targetPos)
//        {
//            isMoving = true;

//            Vector3 start = transform.position;
//            float elapsed = 0f;

//            while (elapsed < moveTime)
//            {
//                transform.position = Vector3.Lerp(start, targetPos, elapsed / moveTime);
//                elapsed += Time.deltaTime;
//                yield return null;
//            }

//            transform.position = targetPos;
//            _gridService.UpdateObjectPosition(this, CurrentGridPosition, targetPos);
//            CurrentGridPosition = targetPos;
            
//            isMoving = false;
//        }
        
//        public void HandleInput(System.Numerics.Vector2 input)
//        {
//        }
        
        
//    }
//}
