using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Work.CIW.Code.Player
{
    #region Interfaces

    public interface IGridDataService
    {
        // Grid System�� �̵� ������ �ο���
        bool CanMoveTo(Vector3Int curPos, Vector3Int dir, out Vector3Int targetPos);

        // �̵� �Ϸ� �� Grid System�� �����͸� ��������
        void UpdateObjectPosition(IGridObject movingObj, Vector3Int oldPos, Vector3Int newPos);

        // Grid System�� Ư�� Grid Object�� ��ġ�� �ʱ�ȭ �Ҷ� ���
        void SetObjectInitialPosition(IGridObject obj, Vector3Int initPos);
    }

    public interface IGridObject
    {
        Vector3Int CurrentGridPosition { get; }

        GameObject GetObject();
    }

    public interface IInteractable
    {
        bool Interact(IGridObject actor, Vector3Int dir);
    }

    public interface IMovement
    {
        void HandleInput(Vector2 input);
    }

    #endregion

    public class PlayerMovement : MonoBehaviour, IMovement, IGridObject
    {
        [Header("Dependencies - DIP")]
        [SerializeField] MonoBehaviour gridServiceMono;
        IGridDataService _gridService;

        [Header("Movement")]
        [SerializeField] float moveTime = 0.15f;

        public Vector3Int CurrentGridPosition { get; private set; }
        public GameObject GetGameObject() => gameObject;

        bool _isMoving = false;

        //[SerializeField] UnityEvent onMoveComplete;

        private void Awake()
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

        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;

            _gridService.SetObjectInitialPosition(this, CurrentGridPosition);
        }

        public void HandleInput(Vector2 input)
        {
            if (_isMoving) return;

            Vector3Int dir = GetDirection(input);
            if (dir == Vector3Int.zero) return;

            Debug.Log($"[PLAYER INPUT] Input received. Direction: {dir}");

            if (_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int targetPos))
            {
                Debug.Log($"[PLAYER MOVEMENT] GridSystem approved move to: {targetPos}");
                StartCoroutine(MoveRoutine(targetPos));
            }
            else
            {
                Debug.LogWarning($"[PLAYER MOVEMENT] Move to {CurrentGridPosition + dir} blocked by GridSystem.");
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

            _isMoving = false;

            //onMoveComplete?.Invoke();
        }

        public GameObject GetObject()
        {
            return gameObject;
        }
    }
}