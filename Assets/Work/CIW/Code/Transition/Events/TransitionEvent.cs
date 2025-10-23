using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CIW.Code.Transition.Events
{
    public struct TransitionEvent : IEvent
    {
        public TransitionType Type;

        public TransitionEvent(TransitionType type)
        {
            Type = type;
        }
    }
}