using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class FillHomeBtn : AbstractFillButtonUI
    {
        [SerializeField] private GameObject homeTransition;
        protected override void HandleBtnClick()
        {
            homeTransition.SetActive(true);
        }
    }
}