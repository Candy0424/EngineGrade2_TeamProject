using System;
using UnityEngine;
using Work.PSB.Code.Test;

namespace Work.CIW.Code.Grid
{
    public class ArrivalTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PSBTestPlayerCode>() != null)
            {
                Debug.Log("도착입니다!");
            }
        }
        
    }
}