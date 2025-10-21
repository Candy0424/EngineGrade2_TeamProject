using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Work.PSB.Code.Test;

namespace Work.CIW.Code.Grid
{
    public class ArrivalTrigger : MonoBehaviour
    {
        public UnityEvent OnArrival;
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("TriggerEnter");
            if (other.gameObject.GetComponent<PSBTestPlayerCode>() != null)
            {
                OnArrival.Invoke();
                Debug.Log("도착입니다!");
            }
        }

        public IEnumerator LobbySceneCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("BookScene");
            
        }
        
        public void LobbyScene()
        {
            StartCoroutine(LobbySceneCoroutine());
        }
        
    }
}