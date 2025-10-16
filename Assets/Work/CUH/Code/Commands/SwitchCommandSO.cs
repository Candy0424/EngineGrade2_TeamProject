using UnityEngine;
using Work.CUH.Code.SwitchSystem;

namespace Work.CUH.Code.Commands
{
    [CreateAssetMenu(fileName = "SwitchCommandSO", menuName = "SO/Commands/Switch", order = 0)]
    public class SwitchCommandSO : BaseCommandSO
    {
        public override bool CanExecute()
        {
            return Commandable is ISwitch;
        }

        public override void Execute()
        {
            ISwitch iSwitch = Commandable as ISwitch;
            iSwitch.ToggleSwitch();
        }

        public override void Undo()
        {
            ISwitch iSwitch = Commandable as ISwitch;
            iSwitch.UndoSwitch();
        }
    }
}