using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.ISC.Code.System
{
    public class TurnManager : MonoBehaviour
    {
        public event Action OnUseTurn;
        
        private List<ITurnAble> _turns;

        private void Awake()
        {
            Initialize();
            Bus<TurnUseEvent>.OnEvent += HandleUseTurn;
        }

        private void OnDestroy()
        {
            Bus<TurnUseEvent>.OnEvent -= HandleUseTurn;
        }
        
        private void Initialize()
        {
            _turns = GetComponentsInChildren<ITurnAble>().ToList();
            _turns.ForEach(c => OnUseTurn += c.TurnUse);
        }
        
        private void HandleUseTurn(TurnUseEvent evt)
        {
            UseTurn();
        }
        
        [ContextMenu("Use Turn")]
        public void UseTurn()
        {
            OnUseTurn?.Invoke();
        }
    }
}