using System;
using Chuh007Lib.Dependencies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.System;

namespace Work.ISC.Code.Managers
{
    [Provide]
    public class TurnCountSystem : MonoBehaviour
    {
        [SerializeField] private int maxTurnCount;
        [SerializeField] private int currentTurnCount;
        
        public int CurrentTurnCount => currentTurnCount;

        public UnityEvent OnTurnZeroEvent;


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
            if (currentTurnCount < 0) return;

            currentTurnCount--;
            
            if (currentTurnCount < 0)
                OnTurnZeroEvent?.Invoke();  
        }
        
        private void TurnGet(TurnGetEvent evt)
        {
            currentTurnCount++;
        }
        
        public void Test()
        {
            Debug.Log("턴 모두 소모");
        }
    }
}