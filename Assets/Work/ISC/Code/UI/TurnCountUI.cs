using System;
using Chuh007Lib.Dependencies;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.CUH.Chuh007Lib.EventBus;
using Work.ISC.Code.Managers;
using Work.PSB.Code.Events;

namespace Work.ISC.Code.UI
{
    public class TurnCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnCountText;
        [SerializeField] private Image image;
        [SerializeField] private GameObject tutorialImage;
        
        [Inject] private TurnCountManager _turnCountManager;
        private int _currentGage;
        private bool _isShaking = false;

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
            Bus<SpikeHitEvent>.OnEvent += HandleSpikeHitText;
        }

        private void OnDestroy()
        {
            _turnCountManager.OnTurnChangeEvent -= UpdateText;
            Bus<SpikeHitEvent>.OnEvent -= HandleSpikeHitText;
        }

        private void HandleSpikeHitText(SpikeHitEvent evt)
        {
            Color originalColor = turnCountText.color;
            Sequence seq = DOTween.Sequence();

            seq.Append(turnCountText.DOColor(Color.red, 0.05f));
            seq.Join(transform.DOShakePosition
            (
                duration: 0.2f,
                strength: new Vector2(15f, 15f),
                vibrato: 30,
                randomness: 90,
                snapping: false,
                fadeOut: true
            ));
            
            if (tutorialImage != null)
            {
                seq.Join(tutorialImage.transform.DOShakePosition
                (
                    duration: 0.2f,
                    strength: new Vector2(15f, 15f),
                    vibrato: 30,
                    randomness: 90,
                    snapping: false,
                    fadeOut: true
                ));
            }

            seq.Append(turnCountText.DOColor(originalColor, 0.1f));
            seq.Play();
        }

        private void UpdateText(int v)
        {
            DOVirtual.Float(_currentGage, v, 0.2f, x => image.material.SetFloat(_gageHash, x))
                .SetEase(Ease.Linear)
                .OnComplete(() => _currentGage = v);

            Sequence seq = DOTween.Sequence();
            seq.Append(DOVirtual.Float(60f, 100f, 0.2f, x => turnCountText.fontSize = x));
            seq.Append(DOVirtual.Float(100f, 60f, 0.2f, x => turnCountText.fontSize = x));
            seq.Play();

            turnCountText.SetText(v.ToString());
        }

        private void Update()
        {
            float ratio = (float)_turnCountManager.CurrentTurnCount / _turnCountManager.MaxTurnCount;
            CheckAndShakeByTurn(ratio);
        }

        private void CheckAndShakeByTurn(float ratio)
        {
            if (_isShaking) return;

            float duration = 0f;
            float strength = 0f;

            if (ratio <= 0.1f)
            {
                duration = 0.3f;
                strength = 4f;
            }
            else if (ratio <= 0.2f)
            {
                duration = 0.25f;
                strength = 2f;
            }
            else if (ratio <= 0.35f)
            {
                duration = 0.2f;
                strength = 0.5f;
            }

            if (duration > 0)
            {
                _isShaking = true;
                turnCountText.rectTransform.DOShakePosition(duration, new Vector3(strength, strength, 0), 25, 90, false, true)
                    .OnComplete(() => _isShaking = false);
            }
        }
        
        
    }
}