using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CUH.Code.GameEvents
{
    public struct TargetPosChangeEvent : IEvent
    {
        public Vector3 position { get; private set; }

        public TargetPosChangeEvent(Vector3 pos)
        {
            position = pos;
        }
    }
}