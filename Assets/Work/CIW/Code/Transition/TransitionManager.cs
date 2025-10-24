using System.Collections.Generic;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CIW.Code.Transition
{
    public enum TransitionType
    {
        None = 0,
        Enter = 1, ReTry = 2, Clear = 3, Fail = 4,
        Default
    }

    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> transitions;

        private void Awake()
        {
            Bus<Events.TransitionEvent>.OnEvent += PlayTransition;
        }

        private void Start()
        {
            foreach (var trn in transitions)
            {
                trn.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Bus<Events.TransitionEvent>.OnEvent -= PlayTransition;
        }

        public void PlayTransition(Events.TransitionEvent evt)
        {
            switch (evt.Type)
            {
                case TransitionType.Enter:
                    
                    break;

                case TransitionType.ReTry:
                    
                    break;

                case TransitionType.Clear:

                    break;

                case TransitionType.Fail:
                    Debug.Log("실패처리");
                    break;

                default:
                    Debug.Log("잘못된 Transition Type");
                    break;
            }


        }
    }
}