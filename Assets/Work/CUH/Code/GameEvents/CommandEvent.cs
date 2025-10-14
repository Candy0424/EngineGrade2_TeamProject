using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.GameEvents
{
    public struct CommandEvent : IEvent
    {
        public BaseCommandSO Command { get; private set; }

        public CommandEvent(BaseCommandSO command)
        {
            Command = command;
        }
    }
}