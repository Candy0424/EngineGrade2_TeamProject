using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.PSB.Code.Events;

namespace Work.PSB.Code.SaveSystem
{
    public class StageClearChecker : MonoBehaviour
    {
        [SerializeField] private int secondStarTurn = 10;
        [SerializeField] private int thirdStarTurn = 5;

        private void OnEnable()
        {
            Bus<StageClearEvent>.OnEvent += HandleStageClear;
        }

        private void OnDisable()
        {
            Bus<StageClearEvent>.OnEvent -= HandleStageClear;
        }

        private void HandleStageClear(StageClearEvent evt)
        {
            bool[] achieved = new bool[3];
            achieved[0] = true;
            if (evt.RemainingTurns >= secondStarTurn) achieved[1] = true;
            if (evt.RemainingTurns >= thirdStarTurn) achieved[2] = true;

            StageProgressManager.Instance.UpdateStage(evt.StageInfo.stageName, true, achieved);
        }
        
    }
}