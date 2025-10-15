using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;

namespace Work.PSB.Code.Commands
{
    [CreateAssetMenu(fileName = "TurnConsumeCommand", menuName = "SO/Commands/TurnConsume", order = 20)]
    public class TurnConsumeCommandSO : BaseCommandSO
    {
        public override bool CanExecute() => true;

        public override void Execute()
        {
            Bus<TurnConsumeOnlyEvent>.Raise(new TurnConsumeOnlyEvent());
        }

        public override void Undo()
        {
            Bus<TurnGetEvent>.Raise(new TurnGetEvent());
        }
        
    }
}