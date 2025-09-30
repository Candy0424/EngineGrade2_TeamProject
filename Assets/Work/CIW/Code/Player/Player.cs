using UnityEngine;

namespace Work.CIW.Code.Player
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public PlayerInputSO InputSO { get; private set; }

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
    }
}