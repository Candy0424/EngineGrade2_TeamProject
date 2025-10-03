using System;
using System.Collections.Generic;
using Chuh007Lib.Dependencies;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.Managers;
using Work.ISC.Code.System;

namespace Work.CUH.Code.Commands
{
    /// <summary>
    /// 모든 커멘드가 실행되는 메니저입니다.
    /// 처리는 이곳의 큐에서 실행되게 됩니다.
    /// </summary>
    [Provide]
    public class CommandManager : MonoBehaviour, ITurnAble
    {
        [SerializeField] private int currentTurnCount = 0;
        
        private Queue<BaseCommandSO> _executionCommands = new Queue<BaseCommandSO>();
        private Stack<BaseCommandSO> _undoCommands = new Stack<BaseCommandSO>();
        private Stack<BaseCommandSO> _redoCommands = new Stack<BaseCommandSO>();
        
        private void Awake()
        {
            Bus<CommandEvent>.OnEvent += HandleCommand;
        }

        private void OnDestroy()
        {
            Bus<CommandEvent>.OnEvent -= HandleCommand;
        }

        public void Undo()
        {
            if (_undoCommands.Count <= 0 || currentTurnCount <= 0) return;
            while (_undoCommands.Count > 0 && _undoCommands.Peek().Tick == currentTurnCount)
            {
                var command = _undoCommands.Pop();
                command.Undo();
            }
            currentTurnCount--;
        }
        
        public void Redo()
        {
            if (_redoCommands.Count <= 0) return;
            
        }

        
        // 플레이어가 행동해 OnUseTurn이 호출되면 호출됨
        // Queue에 플레이어의 행동이 있을거고, 그걸 실행한다.
        // 실행하면서 생기는 커맨드들도 여따가 넣는다.
        public void TurnUse()
        {
            while (_executionCommands.Count > 0)
            {
                BaseCommandSO command = _executionCommands.Dequeue();
                if (command.CanExecute())
                {
                    command.Execute();
                    command.Tick = currentTurnCount;
                    _undoCommands.Push(command);
                }
            }
            currentTurnCount++;
        }
        
        // 커멘드를 넣는 작업
        // 플레이어의 행동 커멘드는 TurnUse보다 먼저 들어와야 한다.
        private void HandleCommand(CommandEvent evt)
        {
            _executionCommands.Enqueue(evt.Command);
        }
    }
}