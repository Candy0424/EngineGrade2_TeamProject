using Work.CUH.Chuh007Lib.EventBus;
using Work.ISC.Code.SO;

namespace Work.PSB.Code.Events
{
    public struct StageClearEvent : IEvent
    {
        public StageInfoSO StageInfo;
        public int RemainingTurns;

        public StageClearEvent(StageInfoSO stageInfo, int remainingTurns)
        {
            StageInfo = stageInfo;
            RemainingTurns = remainingTurns;
        }
        
    }
}