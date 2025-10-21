using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CIW.Code.Camera.Events
{
    public class FloorEvent : IEvent
    {
        // 목표 층 인덱스
        public int TargetIdx { get; private set; }

        // 이동 방향 (1 또는 -1로만)
        public int Direction { get; private set; }

        public FloorEvent(int direction)
        {
            Debug.Log("Floor Event");
            //TargetIdx = targetIdx;
            Direction = direction;
        }
    }
}