using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class ScaleExitBtn : AbstractScaleButtonUI
    {
        protected override void HandleClick()
        {
            base.HandleClick();
            Application.Quit();
        }
    }
}