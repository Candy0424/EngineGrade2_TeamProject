using System;
using Chuh007Lib.Dependencies;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.ISC.Code.Managers
{
    [Provide]
    public class TurnCountManager : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private int maxTurnCount;
        [SerializeField] private int currentTurnCount;
        
        public int CurrentTurnCount
        {
            get => currentTurnCount;
            set
            {
                int v = value;
                if (v != currentTurnCount && v >= 0)
                {
                    currentTurnCount = v;
                    OnTurnChangeEvent?.Invoke(v);
                }
            }
        }

        public Action OnTurnZeroEvent;
        public Action<int> OnTurnChangeEvent;
        
        private void Awake()
        {
            Initialize();
             
        }
        private void Initialize()
        {
            currentTurnCount = maxTurnCount;
            Bus<TurnUseEvent>.OnEvent += TurnUse;
            Bus<TurnGetEvent>.OnEvent += TurnGet;
            Bus<TurnConsumeOnlyEvent>.OnEvent += TurnConsumeOnly;
        }

        private void OnDestroy()
        {
            Bus<TurnUseEvent>.OnEvent -= TurnUse;
            Bus<TurnGetEvent>.OnEvent -= TurnGet;
            Bus<TurnConsumeOnlyEvent>.OnEvent -= TurnConsumeOnly; 
        }


        public void TurnUse(TurnUseEvent evt)
        {
            if (CurrentTurnCount < 0) return;

            CurrentTurnCount--;
            
            if (CurrentTurnCount < 0)
                OnTurnZeroEvent?.Invoke();
        }
        
        private void TurnConsumeOnly(TurnConsumeOnlyEvent evt)
        {
            if (CurrentTurnCount < 0) return;

            CurrentTurnCount--;
            
            if (CurrentTurnCount < 0)
                OnTurnZeroEvent?.Invoke();
        }
        
        private void TurnGet(TurnGetEvent evt)
        {
            CurrentTurnCount++;
        }
        
    }
}