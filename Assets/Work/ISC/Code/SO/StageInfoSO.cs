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
    }
}