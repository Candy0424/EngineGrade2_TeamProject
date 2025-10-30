using Ami.BroAudio;
using UnityEngine;
using UnityEngine.Events;
using Work.CIW.Code.Camera;
using Work.CIW.Code.Player.Event;
using Work.CUH.Chuh007Lib.EventBus;
using Work.PSB.Code.Player;


namespace Work.CIW.Code.Grid
{
    public class ArrivalTrigger : MonoBehaviour
    {
        [SerializeField] FloorTransitionManager floorManager;

        [Header("Sound Setting")]
        [SerializeField] private SoundID endingSound;
        
        public UnityEvent OnArrival;

        bool _isArrival = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isArrival) return;

            if (other.gameObject.GetComponent<PSBTestPlayerCode>() != null)
            {
                _isArrival = true;

                OnArrival.Invoke();
                BroAudio.Play(endingSound);

                Bus<GameClearEvent>.Raise(new GameClearEvent());
            }
        }
        
    }
}