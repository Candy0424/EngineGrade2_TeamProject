using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Work.ISC.Code.System;

namespace Work.CIW.Code.Player
{
    public interface IMovement
    {
        void HandleInput(Vector2 input);
    }

    public class PlayerMovement : MonoBehaviour, IMovement
    {
        [SerializeField] float moveTime = 0.15f;

        bool _isMoving = false;
        Vector3Int _currentGridPos;

        [Header("Events")]
        [SerializeField] UnityEvent OnMoveComplete;

        private void Start()
        {
            _currentGridPos = Vector3Int.RoundToInt(transform.position);
            transform.position = _currentGridPos;
        }

        public void HandleInput(Vector2 input)
        {
            if (_isMoving) return;

            Vector3Int dir = GetDirection(input);
            if (dir != Vector3Int.zero)
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

            OnMoveComplete?.Invoke();
        }
    }
}