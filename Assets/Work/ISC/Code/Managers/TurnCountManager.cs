using System;
using Chuh007Lib.Dependencies;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.ISC.Code.Managers
{
    [Provide]
    public class TurnCountManager : MonoBehaviour
    {
        [SerializeField] private int maxTurnCount;
        [SerializeField] private int currentTurnCount;
        
        public int CurrentTurnCount
        {
            get => currentTurnCount;
            set
            {
                int v = value;
                if (v != currentTurnCount)
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
            Bus<TurnUseEvent>.OnEvent += TurnUse;
            Bus<TurnGetEvent>.OnEvent += TurnGet;
        }

        private void OnDestroy()
        {
            Bus<TurnUseEvent>.OnEvent -= TurnUse;
            Bus<TurnGetEvent>.OnEvent += TurnGet;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            currentTurnCount = maxTurnCount;
        }

        public void TurnUse(TurnUseEvent evt)
        {
            if (CurrentTurnCount < 0) return;

            CurrentTurnCount--;
            
            if (CurrentTurnCount < 0)
                OnTurnZeroEvent?.Invoke();
        }
        
        private void TurnGet(TurnGetEvent evt)
        {
            currentTurnCount++;
        }
    }
}