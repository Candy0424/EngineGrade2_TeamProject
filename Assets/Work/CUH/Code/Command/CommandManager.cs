using System.Collections.Generic;
using Chuh007Lib.Dependencies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Work.CIW.Code.Camera;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.PSB.Code.Test;

namespace Work.CUH.Code.Command
{
    /// <summary>
    /// 모든 커멘드가 실행되는 메니저입니다.
    /// 처리는 이곳의 큐에서 실행되게 됩니다.
    /// </summary>
    [Provide]
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] private float undoCooldown = 0.2f;
        private int _currentTurnCount = 0;

        [Header("Settings")]
        [SerializeField] private int leftUndoCount;
        public UnityEvent ResetEvent;
        
        private Queue<BaseCommand> _executionCommands;
        private Stack<BaseCommand> _undoCommands;
        private float _lastUndoTime;

        [SerializeField] FloorTransitionManager _floorManager;
        [SerializeField] PSBTestPlayerCode _playerCode;
        
        private void Awake()
        {
            _executionCommands = new Queue<BaseCommand>();
            _undoCommands = new Stack<BaseCommand>();
            Bus<CommandEvent>.OnEvent += HandleCommand;
            Bus<TurnUseEvent>.OnEvent += TurnUse;
            Bus<UndoEvent>.OnEvent += HandleUndo;
        }
        
        private void OnDestroy()
        {
            Bus<CommandEvent>.OnEvent -= HandleCommand;
            Bus<TurnUseEvent>.OnEvent -= TurnUse;
            Bus<UndoEvent>.OnEvent -= HandleUndo;
        }
        
        private void HandleUndo(UndoEvent evt)
        {
            Undo();
        }
        
        [ContextMenu("Undo")]
        public void Undo()
        {
            if (_floorManager.IsBookTurned) return;

            Debug.Log($"Undo 안에 들어옴 : {_floorManager.IsBookTurned}");

            if (_undoCommands.Count <= 0 || _currentTurnCount <= 0) return;
            if (!_undoCommands.Peek().CanExecute()) return;
            // if (leftUndoCount <= 0) return;
            bool undo = false;
            while (_undoCommands.Count > 0 && _undoCommands.Peek().Tick >= _currentTurnCount)
            {
                undo = true;
                _undoCommands.Pop().Undo();
            }
            if (undo)
            {
                leftUndoCount--;
                _currentTurnCount--;
                Debug.Log("Undo 턴 해줄게");
                Bus<TurnGetEvent>.Raise(new TurnGetEvent());
            }
        }
        
        // 플레이어가 행동해 OnUseTurn이 호출되면 호출됨
        // Queue에 플레이어의 행동이 있을거고, 그걸 실행한다.
        // 실행하면서 생기는 커맨드들도 여따가 넣는다.
        public void TurnUse(TurnUseEvent evt)
        {
            _currentTurnCount++;
            while (_executionCommands.Count > 0)
            {
                BaseCommand command = _executionCommands.Dequeue();
                if (command.CanExecute())
                {
                    command.Tick = _currentTurnCount;
                    command.Execute();
                    _undoCommands.Push(command);
                }
            }
        }


        
        // 커멘드를 넣는 작업
        // 플레이어의 행동 커멘드는 TurnUse보다 먼저 들어와야 한다.
        private void HandleCommand(CommandEvent evt)
        {
            _executionCommands.Enqueue(evt.Command);
        }

        private void Update()
        {
            if (Keyboard.current.zKey.isPressed && Time.time > undoCooldown + _lastUndoTime && !_floorManager.IsBookTurned && !_playerCode.IsInputLocked) // 지금 넘어가는 중인지
            {
                _lastUndoTime = Time.time;
                Debug.Log($"Z키 눌렀으니 Undo 실행함 : {_playerCode.IsInputLocked}");
                Undo();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetEvent?.Invoke();
            }
        }
    }
}