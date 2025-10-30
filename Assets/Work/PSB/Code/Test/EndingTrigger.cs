using UnityEngine;
using UnityEngine.Events;

namespace Work.PSB.Code.Test
{
    public class EndingTrigger : MonoBehaviour
    {
        public UnityEvent OnEnd;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CharacterController>() != null)
            {
                Debug.Log("엔딩 트리거 발동!");
                OnEnd?.Invoke();
            }
        }
        
    }
}