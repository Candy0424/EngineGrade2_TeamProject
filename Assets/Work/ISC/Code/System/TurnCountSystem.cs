using Chuh007Lib.Dependencies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Work.ISC.Code.System;

namespace Work.ISC.Code.Managers
{
    [Provide]
    public class TurnCountSystem : MonoBehaviour, ITurnAble
    {
        [SerializeField] private int maxTurnCount;
        [SerializeField] private int currentTurnCount;
        
        public int CurrentTurnCount => currentTurnCount;

        public UnityEvent OnTurnZeroEvent;
        
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            currentTurnCount = maxTurnCount;
        }

        public void TurnUse()
        {
            if (currentTurnCount < 0) return;

            currentTurnCount--;
            
            if (currentTurnCount < 0)
                OnTurnZeroEvent?.Invoke();  
        }

        public void Test()
        {
            Debug.Log("턴 모두 소모");
        }
    }
}