using System.Collections;
using UnityEngine;

namespace Work.PSB.Code.Test
{
    public class BlockPushTest : MonoBehaviour
    {
        [SerializeField] private float moveTime = 0.15f;
        [SerializeField] private bool canMoveBlock = true;
        private bool _isMoving = false;
        private Vector3Int _gridPos;

        private void Start()
        {
            _gridPos = Vector3Int.RoundToInt(transform.position);
            transform.position = _gridPos;
        }

        public bool CanMove(Vector3Int dir)
        {
            if (!canMoveBlock) return false;
            
            Vector3Int targetPos = _gridPos + dir;
            
            Collider[] hits = Physics.OverlapSphere(targetPos, 0.1f);
            foreach (Collider hit in hits)
            {
                if (hit.GetComponent<BlockPushTest>() != null)
                    return false;
                if (hit.CompareTag("Wall") || hit.CompareTag("Spike"))
                    return false;
            }

            return true;
        }

        public IEnumerator MoveRoutine(Vector3Int dir)
        {
            if (_isMoving) yield break;

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
            _gridPos = Vector3Int.RoundToInt(end);

            _isMoving = false;
        }
        
    }
}