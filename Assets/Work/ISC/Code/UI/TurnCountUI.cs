using System;
using Chuh007Lib.Dependencies;
using TMPro;
using UnityEngine;
using Work.ISC.Code.Managers;

namespace Work.ISC.Code.UI
{
    public class TurnCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnCountText;
        
        [Inject] private TurnCountManager _turnCountManager;
        
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            UpdateText(_turnCountManager.CurrentTurnCount);
            
            _turnCountManager.OnTurnChangeEvent += UpdateText;
        }

        private void OnDestroy()
        {
            _turnCountManager.OnTurnChangeEvent -= UpdateText;
        }

        private void UpdateText(int v)
        {
            turnCountText.SetText(v.ToString());
        }
    }
}