using Chuh007Lib.Dependencies;
using UnityEngine;
using Work.ISC.Code.Managers;

namespace Work.ISC.Code.UI
{
    public class TurnCountUI : MonoBehaviour
    {
        [Inject] private TurnCountManager _turnCountManager;
    }
}