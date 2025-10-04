using UnityEngine;

namespace Work.CIW.Code.Grid
{
    public class StairTrigger : MonoBehaviour
    {
        [Tooltip("이걸 밟았을 때 이동할 목표 층의 y 좌표")]
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