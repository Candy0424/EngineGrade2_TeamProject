using UnityEngine;
using Work.ISC.Code.Managers;

namespace Work.CIW.Code.Player
{
    public interface ITurnService
    {
        bool HasTurnRemaining { get; }

        void UseTurn();
    }

    public class TurnSystemAdapter : MonoBehaviour, ITurnService
    {
        [Header("Target (Adapter Pattern)")]
        [SerializeField] TurnCountSystem turnCntSystem;

        public bool HasTurnRemaining
        {
            get
            {
                if (turnCntSystem == null) return true;
                return turnCntSystem.CurrentTurnCount > 0;
            }
        }

        public void UseTurn()
        {
            if (turnCntSystem == null) return;

            turnCntSystem.TurnUse();
        }
    }
}