using UnityEngine;

namespace Work.CIW.Code.Player
{
    [CreateAssetMenu(fileName = "PLayerMoveCommand", menuName = "SO/Player/Command")]
    public class MovePlayerCommandSO : MovementCommand
    {
        public override bool Execute(IMovement movement, Vector3Int direction)
        {
            return movement.StartMove(direction);
        }
    }
}