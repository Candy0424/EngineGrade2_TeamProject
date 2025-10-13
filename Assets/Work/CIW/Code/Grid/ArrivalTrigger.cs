using UnityEngine;
using Work.CIW.Code.Player;

namespace Work.CIW.Code.Grid
{
    public class ArrivalTrigger : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.GetComponent<Player.Player>() != null)
            {
                Debug.Log("도착입니다!");
            }
        }
    }
}