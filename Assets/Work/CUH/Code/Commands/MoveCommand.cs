using UnityEngine;

namespace Work.CUH.Code.Commands
{
    [CreateAssetMenu(fileName = "MoveCommand", menuName = "SO/Commands/Move", order = 0)]
    public class MoveCommand : BaseCommandSO
    {
        public override bool CanExecute()
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}