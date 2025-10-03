using UnityEngine;

namespace Work.CUH.Code.Commands
{
    public abstract class BaseCommandSO : ScriptableObject, ICommand
    {
        public AbstractCommandable Commandable { get; protected set; } // 커멘드를 수행하는 놈
        public int Tick { get; set; } // 이게 실행된 턴
        public abstract bool CanExecute(); // 수행 가능한가?
        public abstract void Execute(); // 실제 수행
        public abstract void Undo();
    }
}