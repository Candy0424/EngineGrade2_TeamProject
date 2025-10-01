namespace Work.CUH.Code.Commands
{
    public struct CommandContext // 커멘드 작동을 위해 필요한 데이터들
    {
        public AbstractCommandable Commandable { get; private set; } // 커멘드를 수행하는 놈

        public CommandContext(AbstractCommandable commandable)
        {
            Commandable = commandable;
        }
    }
}