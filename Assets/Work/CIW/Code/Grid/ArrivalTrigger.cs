using Ami.BroAudio;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Player.Event;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.Managers;
using Work.ISC.Code.SO;
using Work.PSB.Code.Player;
using Chuh007Lib.Dependencies;
using Work.PSB.Code.Events;

namespace Work.CIW.Code.Grid
{
    public class ArrivalTrigger : MonoBehaviour
    {
        [Header("Stage Info")]
        [SerializeField] private StageInfoSO stageInfo;
        [SerializeField] private TurnCountManager turnCountManager;

        [Header("Sound Setting")]
        [SerializeField] private SoundID endingSound;

        private bool _isArrival;

        public UnityEvent OnArrival;

        private void Awake()
        {
            if (turnCountManager == null)
                Debug.LogWarning("[ArrivalTrigger] TurnCountManager not found.");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isArrival) return;
            if (!other.TryGetComponent(out PSBTestPlayerCode player)) return;

            _isArrival = true;
            BroAudio.Play(endingSound);

            int remainingTurns = turnCountManager != null ? turnCountManager.CurrentTurnCount : 0;

            Bus<StageClearEvent>.Raise(new StageClearEvent(stageInfo, remainingTurns));

            OnArrival?.Invoke();
        }
        
        
    }
}