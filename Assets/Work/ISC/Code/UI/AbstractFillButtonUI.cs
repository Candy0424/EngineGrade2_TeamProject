using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work.ISC.Code.UI
{
    public abstract class AbstractFillButtonUI : MonoBehaviour
    {
        [SerializeField] private RectTransform fillBar;
        [SerializeField] private Button btn;

        private float _curGage;
        
        protected bool isHold = false;
        
        private Tween _tween;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        { 
            fillBar.localScale = new Vector3(0, 1, 1);
        }

        private void Update()
        {
            if (isHold)
            {
                _curGage += 0.01f;
            }
            else
                _curGage = .0f;
        }

        private void LateUpdate()
        {
            SetPercent();
        }   

        private void SetPercent()
        {
            if (_tween.IsActive()) _tween.Kill();
            _tween = fillBar.DOScaleX(_curGage, _curGage <= .0f ? 0 : 1.5f)
                .SetEase(Ease.Linear).SetUpdate(true);
            if (fillBar.localScale.x >= 1f)
            {
                isHold = false;
                _curGage = .0f;
                HandleBtnClick();
            }
        }

        protected abstract void HandleBtnClick();

        public virtual void HandleDown() => isHold = true;

        public virtual void HandleUp() => isHold = false;
    }
}