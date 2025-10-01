using UnityEngine;

namespace Work.CUH.Code.Commands
{
    // 커멘드들이 상속받는 클래스
    public abstract class BaseCommandSO : ScriptableObject, ICommand
    {
        public abstract bool CanHandle(CommandContext context);

        public abstract void Handle(CommandContext context);

        public void Undo()
        {
            
        }
    }
}