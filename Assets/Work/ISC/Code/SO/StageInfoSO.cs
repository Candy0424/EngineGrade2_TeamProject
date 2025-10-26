using UnityEngine;

namespace Work.ISC.Code.SO
{
    [CreateAssetMenu(fileName = "New StageLevelInfo", menuName = "SO/StageLevelInfo", order = 0)]
    public class StageInfoSO : ScriptableObject
    {
        public int level;
        public Sprite stageImg;
        public string stageName;
        
        [TextArea]
        public string description;

        [Header("별 기준 적는 곳")]
        [TextArea] public string firstStd;
        [TextArea] public string secondStd;
        [TextArea] public string thirdStd;
    }
}