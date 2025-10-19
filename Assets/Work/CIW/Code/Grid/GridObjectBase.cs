using UnityEngine;

namespace Work.CIW.Code.Grid
{
    public abstract class GridObjectBase : MonoBehaviour
    {
        public abstract Vector3Int CurrentGridPosition { get; set; }

        public Transform Transform => transform;

        // 셀 점유/해제 시 필요한 로직을 구현하시면 됩니다.
        public abstract void OnCellDeoccupied();
        public abstract void OnCellOccupied(Vector3Int newPos);

        /// <summary>
        /// 공통 로직 구현하면 됨.
        /// </summary>
        protected virtual void Initialize()
        {

        }
    }
}