using Unity.Cinemachine;
using UnityEngine;

namespace Work.PSB.Code.Feedbacks
{
    public class CameraShakeFeedback : Feedback
    {
        [SerializeField] private float impulseForce = 0.6f;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        public override void CreateFeedback()
        {
            impulseSource.GenerateImpulse(impulseForce);
        }

        public override void StopFeedback()
        {
            //do nothing
        }
        
    }
}