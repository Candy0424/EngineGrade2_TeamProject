using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Work.CIW.Code.Camera;
using Work.CIW.Code.Player.Event;
using Work.CUH.Chuh007Lib.EventBus;
using Work.PSB.Code.Test;

namespace Work.CIW.Code.Grid
{
    public class ArrivalTrigger : MonoBehaviour
    {
        [SerializeField] FloorTransitionManager floorManager;

        // event bus로 변환해서 해야해
        public UnityEvent OnArrival;

        bool _isArrival = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isArrival) return;

            Debug.Log("TriggerEnter");
            if (other.gameObject.GetComponent<PSBTestPlayerCode>() != null)
            {
                _isArrival = true;

                OnArrival.Invoke();

                Bus<GameClearEvent>.Raise(new GameClearEvent());

                Debug.Log("도착입니다!");
            }
        }

        public IEnumerator LobbySceneCoroutine()
        {
            //floorManager.SetBookState(4);

            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("BookScene");
            
        }
        
        public void LobbyScene()
        {
            StartCoroutine(LobbySceneCoroutine());
        }
        
    }
}