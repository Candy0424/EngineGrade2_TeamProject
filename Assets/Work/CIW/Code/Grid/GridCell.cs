using UnityEngine;
using Work.CIW.Code.Player;

namespace Work.CIW.Code.Grid
{
    /// <summary>
    /// 3���� ������ �� ĭ�� ��Ÿ����, �ش� ĭ�� ������ �������ش�
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        [field : SerializeField] public Vector3Int Coordinates { get; private set; }

        // �ش� ĭ�� �����ϰ� �ִ� ������Ʈ�� ������ �����Ѵ�
        public IGridObject Occupant { get; private set; }

        public bool IsOccupant => Occupant != null;

        // ĭ�� �Ӽ��� �������� �ʵ���
        public bool IsWalkable = true;

        public void InitializeCoodinates(Vector3Int pos)
        {
            Coordinates = pos;
        }

        /// <summary>
        /// Grid System�� ���� �����ڸ� ����/���� �� ���ȴ�
        /// </summary>
        /// <param name="obj"></param>
        public void SetOccupant(IGridObject obj)
        {
            Occupant = obj;
        }
    }
}