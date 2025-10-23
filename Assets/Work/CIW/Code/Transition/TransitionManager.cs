using System.Collections.Generic;
using UnityEngine;

namespace Work.CIW.Code.Transition
{
    public enum TransitionType
    {
        Enter, ReTry, End
    }

    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> transitions;

        public void PlayTransition(TransitionType type)
        {
            switch (type)
            {
                case TransitionType.Enter:

                    break;

                case TransitionType.ReTry:

                    break;

                case TransitionType.End:

                    break;
            }
        }
    }
}