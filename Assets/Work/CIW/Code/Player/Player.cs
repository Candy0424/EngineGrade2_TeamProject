using UnityEngine;
using Work.CIW.Code.Grid;

namespace Work.CIW.Code.Player
{
    public class Player : GridObjectBase
    {
        [field: SerializeField] public PlayerInputSO InputSO { get; private set; }

        [SerializeField] Vector3Int initialPosition = Vector3Int.zero;
        public override Vector3Int CurrentGridPosition { get; set; }

        IMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<IMovement>();
        }

        private void OnEnable()
        {
            if (InputSO != null)
                InputSO.OnMovement += HandleMove;
        }

        private void OnDisable()
        {
            if (InputSO != null)
                InputSO.OnMovement -= HandleMove;
        }

        private void HandleMove(Vector2 input)
        {
            _movement?.HandleInput(input);
        }

        public override void OnCellDeoccupied()
        {
            // 셀에서 나갈 때 필요한 로직
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
            CurrentGridPosition = newPos;
            transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
    }
}