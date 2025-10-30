using DG.Tweening;
using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class ScaleOptionBtn : AbstractScaleButtonUI
    {
        [SerializeField] private RectTransform opTransform;

        private bool _isOpen;

        protected override void Initialize()
        {
            base.Initialize();
            _isOpen = false;
            opTransform.localScale = new Vector3(0, 1, 1);
        }

        protected override void HandleClick()
        {
            base.HandleClick();
            _isOpen = !_isOpen;
            float value = _isOpen ? 1f : 0f;
            opTransform.DOScaleX(value, 0.2f).SetEase(Ease.Linear);
        }
    }
}