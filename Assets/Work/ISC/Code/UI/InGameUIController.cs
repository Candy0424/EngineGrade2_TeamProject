using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.ISC.Code.UI
{
    public class InGameUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform inGamedUI;
        private bool _isOpen;
        private Tween _tween;

        private void Awake()
        {
            inGamedUI.localScale = new Vector3(1, 0, 1);
            _isOpen = false;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                _isOpen = !_isOpen;
            }

            UpdatePanel(_isOpen);
        }

        public void UpdatePanel(bool isOpen)
        {
            int value = isOpen ? 1 : 0;
            Time.timeScale = isOpen ? 0 : 1;
            if (_tween.IsActive()) _tween.Kill();
            _tween = inGamedUI.DOScaleY(value, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
            if (_tween.IsActive()) _tween.Kill();
        }
    }
}