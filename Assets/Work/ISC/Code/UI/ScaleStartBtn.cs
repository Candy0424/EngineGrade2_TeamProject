using Ami.BroAudio;
using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class ScaleStartBtn : AbstractScaleButtonUI
    {
        [SerializeField] private GameObject startTransition;
        
        protected override void HandleClick()
        {
            base.HandleClick();
            startTransition.SetActive(true);
        }
    }
}