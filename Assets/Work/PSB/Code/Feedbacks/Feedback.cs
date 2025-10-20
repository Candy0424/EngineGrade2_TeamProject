using UnityEngine;

namespace Work.PSB.Code.Feedbacks
{
    public abstract class Feedback : MonoBehaviour
    {
        public abstract void CreateFeedback();
        public abstract void StopFeedback();
    }
}