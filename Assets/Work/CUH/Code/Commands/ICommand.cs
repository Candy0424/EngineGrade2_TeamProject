namespace Work.CUH.Code.Commands
{
    /// <summary>
    /// 커맨드는 상세 동작은 모르나, 어떤 대상이 이러한 메서드를 갖고 있다 전도는 앎(추상화해서).
    /// 그걸 실행시키는 역할이 커맨드의 역할
    /// </summary>
    public interface ICommand
    {
        public bool CanHandle(CommandContext context); // 수행 가능한가?
        public void Handle(CommandContext context); // 실제 수행
        public void Undo();
    }
}