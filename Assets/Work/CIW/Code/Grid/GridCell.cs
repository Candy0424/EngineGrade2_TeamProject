using UnityEngine;

namespace Work.CIW.Code.Grid
{
    /// <summary>
    /// 3차원 격자의 한 칸을 나타내며, 해당 칸의 정보를 저장해준다
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        [field : SerializeField] public Vector3Int Coordinates { get; private set; }

        // 해당 칸을 점유하고 있는 오브젝트의 참조를 저장한다
        public GridObjectBase Occupant { get; private set; }

        public bool IsOccupant => Occupant != null;

        // 칸의 속성을 정의해줄 필드임
        public bool IsWalkable = true;

        public void InitializeCoodinates(Vector3Int pos)
        {
            Coordinates = pos;
        }

        /// <summary>
        /// Grid System이 셀의 점유자를 설정/해제 시 사용된다
        /// </summary>
        /// <param name="obj"></param>
        public void SetOccupant(GridObjectBase obj)
        {
            Occupant = obj;
        }
    }
}