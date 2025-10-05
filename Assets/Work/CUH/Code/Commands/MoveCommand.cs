using UnityEngine;
using Work.CIW.Code.Player;
using Work.CUH.Code.Test;

namespace Work.CUH.Code.Commands
{
    // 주의사항 : 커멘드를 쓸때는 반드시 Instance로 생성해서 써야한다. 안 그러면 꼬임.
    [CreateAssetMenu(fileName = "MoveCommand", menuName = "SO/Commands/Move", order = 0)]
    public class MoveCommand : BaseCommandSO
    {
        public Vector2 Dir { get; set; }
        
        public override bool CanExecute()
        {
            // 이 부분은 커멘드마다 달라짐. 적당히 조건 맞춰야한다.
            // 이거같은 경우는 IMoveable해야 하고, 그놈이 움직이고 있지 않아야 작동 가능하다.
            return Commandable is IMoveableTest moveable
                && moveable.isMoving == false;
        }
        
        // 실행이나 언도는 내부구현 적당히 하면 됨. (이래서 인터페이스 분리가 중요. 안하면 강한의존)
        public override void Execute()
        {
            IMoveableTest movement = Commandable as IMoveableTest;
            movement.HandleInput(Dir); // CanExecute에서 검사했으니 안해도 됨.
        }
        
        public override void Undo()
        {
            IMoveableTest movement = Commandable as IMoveableTest;
            movement.HandleInput(-Dir); // CanExecute에서 검사했으니 안해도 됨.
        }
    }
}