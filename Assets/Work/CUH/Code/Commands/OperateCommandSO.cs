using UnityEngine;
using Work.CUH.Code.SwitchSystem;

namespace Work.CUH.Code.Commands
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "SO/Commands/Operate", order = 0)]
    public class OperateCommandSO : BaseCommandSO
    {
        public override bool CanExecute()
        {
            return Commandable is ISwitch;
        }

        public override void Execute()
        {
            ISwitch iSwitch = Commandable as ISwitch;
            iSwitch.SwitchOn();
        }

        public override void Undo()
        {
            ISwitch iSwitch = Commandable as ISwitch;
            iSwitch.SwitchOff();
        }
    }
}