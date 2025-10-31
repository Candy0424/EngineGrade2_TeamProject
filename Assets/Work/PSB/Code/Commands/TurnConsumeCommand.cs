using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Command;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;

namespace Work.PSB.Code.Commands
{
    public class TurnConsumeCommand : BaseCommand
    {
        public TurnConsumeCommand(ICommandable commandable = null) : base(commandable)
        {
        }

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