using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Work.ISC.Code.System
{
    public class TurnManager : MonoBehaviour
    {
        public event Action OnUseTurn;
        
        private List<ITurnAble> _turns;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _turns = GetComponentsInChildren<ITurnAble>().ToList();
            _turns.ForEach(c => OnUseTurn += c.TurnUse);
        }
        
        [ContextMenu("Use Turn")]
        public void HandleUseTurn()
        {
            OnUseTurn?.Invoke();
        }
    }
}