//using UnityEngine;
//using Work.CIW.Code;
//using Work.CIW.Code.Player;
//using Work.CUH.Chuh007Lib.EventBus;
//using Work.CUH.Code.Commands;
//using Work.CUH.Code.GameEvents;
//using Work.ISC.Code.System;

//namespace Work.CUH.Code.Test
//{
//    public class TestPlayer : MonoBehaviour
//    {
//        [field: SerializeField] public PlayerInputSO InputSO { get; private set; }

//        [SerializeField] private MoveCommand moveCommand; // 별로 마음에 들진 않는데, 이게 최선인듯.
        
//        private TestPlayerMovement _movement;

//        private void Awake()
//        {
//            _movement = GetComponent<TestPlayerMovement>();
//        }
        
//        private void OnEnable()
//        {
//            if (InputSO != null)
//                InputSO.OnMovement += HandleMove;
//        }

//        private void OnDisable()
//        {
//            if (InputSO != null)
//                InputSO.OnMovement -= HandleMove;
//        }

//        // 테스트용이라 블록 막힘이나 밀기 생각 안함.
//        // 못가는데 누른것도 기록할거면 Movement에서 막아버리면 되고, 그건 기록 안할거면 여기서 막아야함.
//        private void HandleMove(Vector2 dir)
//        {
//            if (_movement == null) return;
//            var command = Instantiate(moveCommand); // 커멘드 복사본 만들기(SO는 이리 해야함.)
//            // 데이터 넣기
//            command.Commandable = _movement;
//            command.Dir = dir;
//            if (command.CanExecute())
//            {
//                Bus<CommandEvent>.Raise(new CommandEvent(command)); // 커멘드 추가시키기
//                // 턴 소모 알림(플레이어 입력이니깐, 블럭 밀리는 것 같은건 필요 없음)
//                Bus<TurnUseEvent>.Raise(new TurnUseEvent()); 
//            }
//        }
//    }
//}