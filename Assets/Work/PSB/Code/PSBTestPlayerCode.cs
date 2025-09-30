using System.Collections;
using UnityEngine;
using Work.CIW.Code.Player;

namespace Work.PSB.Code
{
    public class PSBTestPlayerCode : MonoBehaviour, IMovement
    {
        [SerializeField] float moveTime = 0.15f;

        bool _isMoving = false;
        Vector3Int _currentGridPos;

        private void Start()
        {
            _currentGridPos = Vector3Int.RoundToInt(transform.position);
            transform.position = _currentGridPos;
        }

        public void HandleInput(Vector2 input)
        {
            if (_isMoving) return;

            Vector3Int dir = GetDirection(input);
            if (dir == Vector3Int.zero) return;
            
            Vector3Int nextPos = _currentGridPos + dir;
            Collider[] hits = Physics.OverlapSphere(nextPos, 0.1f);
            foreach (Collider hit in hits)
            {
                BlockPushTest block = hit.GetComponent<BlockPushTest>();
                if (block != null)
                {
                    if (block.CanMove(dir))
                    {
                        StartCoroutine(block.MoveRoutine(dir));
                    }
                    else
                    {
                        return;
                    }
                }
            }
            
            StartCoroutine(MoveRoutine(dir));
        }

        private Vector3Int GetDirection(Vector2 input)
        {
            if (input.y > 0.5f) return Vector3Int.forward;
            if (input.y < -0.5f) return Vector3Int.back;
            if (input.x > 0.5f) return Vector3Int.right;
            if (input.x < -0.5f) return Vector3Int.left;
            return Vector3Int.zero;
        }

        private IEnumerator MoveRoutine(Vector3Int dir)
        {
            _isMoving = true;

            Vector3 start = transform.position;
            Vector3 end = start + dir;

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            _currentGridPos = Vector3Int.RoundToInt(end);

            _isMoving = false;
        }
        
    }
}