using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.UI
{
    public class ResetUI : MonoBehaviour
    {
        [SerializeField] private GameObject _resetUI;

        private bool _isActive;
        private void Awake()
        {
            Bus<ResetUIOpenEvent>.OnEvent += HandleResetUI;
        }

        private void OnDestroy()
        {
            Bus<ResetUIOpenEvent>.OnEvent -= HandleResetUI;
        }

        private void HandleResetUI(ResetUIOpenEvent evt)
        {
            _resetUI.SetActive(!_isActive);
            _isActive = !_isActive;
        }

        public void Reset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void Close()
        {
            Bus<ResetUIOpenEvent>.Raise(new ResetUIOpenEvent());
        }
    }
}