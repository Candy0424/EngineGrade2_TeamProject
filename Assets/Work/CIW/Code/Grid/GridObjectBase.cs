using UnityEngine;

namespace Work.CIW.Code.Grid
{
    public abstract class GridObjectBase : MonoBehaviour
    {
        public abstract Vector3Int CurrentGridPosition { get; set; }

        public Transform Transform => transform;

        // �� ����/���� �� �ʿ��� ������ �����Ͻø� �˴ϴ�.
        public abstract void OnCellDeoccupied();
        public abstract void OnCellOccupied(Vector3Int newPos);

        /// <summary>
        /// ���� ���� �����ϸ� ��.
        /// </summary>
        protected virtual void Initialize()
        {

        }
    }
}