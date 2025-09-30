using System.Collections;
using UnityEngine;

namespace Work.PSB.Code
{
    public class BlockPullTest : MonoBehaviour
    {
        [SerializeField] private float moveTime = 0.15f;
        private bool _isMoving = false;
        private Vector3Int _gridPos;

        private void Start()
        {
            _gridPos = Vector3Int.RoundToInt(transform.position);
            transform.position = _gridPos;
        }

        public bool CanMove(Vector3Int dir)
        {
            Vector3Int target = _gridPos + dir;
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