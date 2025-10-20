using System;
using UnityEngine;
using UnityEngine.Events;
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
        
    }
}