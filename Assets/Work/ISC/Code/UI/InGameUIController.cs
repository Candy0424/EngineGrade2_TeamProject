using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.ISC.Code.UI
{
    public class InGameUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform inGamedUI;
        private bool _isOpen;
        private bool _prev;
        private Tween _tween;

        private void Awake()
        {
            inGamedUI.localScale = new Vector3(1, 0, 1);
            _isOpen = false;
            SetCursorLock();
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
            _isOpen = isOpen;
            if (_isOpen != _prev)
            {
                SetCursorLock();
                _prev = _isOpen;
            }
            int value = isOpen ? 1 : 0;
            Time.timeScale = isOpen ? 0 : 1;
            if (_tween.IsActive()) _tween.Kill();
            _tween = inGamedUI.DOScaleY(value, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
        }
        
        private void SetCursorLock()
        {
            if (!_isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
            if (_tween.IsActive()) _tween.Kill();
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}