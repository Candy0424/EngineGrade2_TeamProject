using System;
using Chuh007Lib.Dependencies;
using DG.Tweening;
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
            Sequence seq = DOTween.Sequence();
            seq.Append(DOVirtual.Float(60f, 100f, 0.2f, x => turnCountText.fontSize = x));
            seq.Append(DOVirtual.Float(100f, 60f, 0.2f, x => turnCountText.fontSize = x));
            seq.Play();
            turnCountText.SetText(v.ToString());
        }
    }
}