using UnityEngine;

namespace Work.CIW.Code.Grid
{
    public class StairTrigger : MonoBehaviour
    {
        [Tooltip("�̰� ����� �� �̵��� ��ǥ ���� y ��ǥ")]
        [SerializeField] int targetFloorY;

        //public Vector3Int StairPosition { get; private set; }
        //private void Start()
        //{
        //    StairPosition = Vector3Int.RoundToInt(transform.position);
        //}

        public int GetTargetY()
        {
            return targetFloorY;
        }
    }
}