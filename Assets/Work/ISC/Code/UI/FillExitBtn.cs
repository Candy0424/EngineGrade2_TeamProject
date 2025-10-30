using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class FillExitBtn : AbstractFillButtonUI
    {
        [SerializeField] private GameObject homeTransition;

        protected override void HandleBtnClick()
        {
            homeTransition.SetActive(true);
        }
        
        public void ExitGame() => Application.Quit();
    }
}