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

        private void Start()
        {
            
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
            throw new System.NotImplementedException();
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
            throw new System.NotImplementedException();
        }
    }
}