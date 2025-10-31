using echo17.EndlessBook.Demo01;
using Work.CUH.Code.Command;

namespace Work.CIW.Code.Camera
{
    public class BookTurnCommand : BaseCommand
    {
        FloorTransitionManager _manager;
        Demo01 _bookController;
        int _nextFloorIdx;
        int _direction;
        int _prevFloorIdx;

        public BookTurnCommand(FloorTransitionManager manager, Demo01 bookController, int nextIdx, int direction, int currentIdx) : base(manager) 
        {
            _manager = manager;
            _bookController = bookController;
            _nextFloorIdx = nextIdx;
            _direction = direction;
            _prevFloorIdx = currentIdx;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            //_manager.StartCoroutine(_manager.TransitionSequence(_nextFloorIdx));
        }

        public override void Undo()
        {
            //int undoDir = -_direction;

            //_manager.StartCoroutine(_manager.UndoTransitionSequence(_prevFloorIdx, undoDir));
        }
    }
}