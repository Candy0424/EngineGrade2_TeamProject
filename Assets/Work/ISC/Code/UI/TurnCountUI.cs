using System;
using Chuh007Lib.Dependencies;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.ISC.Code.Managers;

namespace Work.ISC.Code.UI
{
    public class TurnCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnCountText;
        [SerializeField] private Image image;
        
        [Inject] private TurnCountManager _turnCountManager;
        private int _currentGage;

        private readonly int _gageHash = Shader.PropertyToID("_gage");
        private readonly int _maxGageHash = Shader.PropertyToID("_maxgage");
        
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            int curCnt = _turnCountManager.CurrentTurnCount;
            _currentGage = curCnt;
            image.material.SetInt(_maxGageHash, curCnt);
            UpdateText(curCnt);
            
            _turnCountManager.OnTurnChangeEvent += UpdateText;
        }

        private void OnDestroy()
        {
            _turnCountManager.OnTurnChangeEvent -= UpdateText;
        }

        private void UpdateText(int v)
        {
            DOVirtual.Float(_currentGage, v, 0.2f, x => image.material.SetFloat(_gageHash, x)).SetEase(Ease.Linear);
            Sequence seq = DOTween.Sequence();
            seq.Append(DOVirtual.Float(60f, 100f, 0.2f, x => turnCountText.fontSize = x));
            seq.Append(DOVirtual.Float(100f, 60f, 0.2f, x => turnCountText.fontSize = x));
            seq.Play();
            turnCountText.SetText(v.ToString());
            
            _currentGage = v;
        }
    }
}