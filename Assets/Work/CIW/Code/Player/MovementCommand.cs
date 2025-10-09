using UnityEngine;

namespace Work.CIW.Code.Player
{
    public abstract class MovementCommand : ScriptableObject
    {
        public abstract bool Execute(IMovement movement, Vector3Int direction);
    }
}