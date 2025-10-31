using UnityEngine;

namespace Work.CIW.Code.Grid
{
    public class StairTrigger : MonoBehaviour
    {
        [Tooltip("�̰� ����� �� �̵��� ��ǥ ���� y ��ǥ")]
        [SerializeField] int targetFloorY;
        
        public int GetTargetY()
        {
            return targetFloorY;
        }
    }
}