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
                OnEnd?.Invoke();
            }
        }
        
    }
}