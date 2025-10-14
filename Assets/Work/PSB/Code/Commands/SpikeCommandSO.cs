using UnityEngine;
using Work.CUH.Code.Commands;
using Work.PSB.Code.Test;

namespace Work.PSB.Code.Commands
{
    [CreateAssetMenu(fileName = "SpikeCommand", menuName = "SO/Commands/Spike", order = 10)]
    public class SpikeCommandSO : BaseCommandSO
    {
        private bool _wasRaisedBefore;

        public override bool CanExecute()
        {
            return Commandable is SpikeController;
        }

        public override void Execute()
        {
            if (Commandable is SpikeController spike)
            {
                _wasRaisedBefore = spike.IsRaised; 
                spike.ToggleSpikeCommanded();
            }
        }

        public override void Undo()
        {
            if (Commandable is SpikeController spike)
            {
                if (spike.IsRaised != _wasRaisedBefore)
                    spike.ToggleSpikeCommanded();
            }
        }
        
    }
}