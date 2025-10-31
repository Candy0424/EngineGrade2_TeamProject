using Ami.BroAudio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Work.ISC.Code.UI
{
    public abstract class AbstractFillButtonUI : MonoBehaviour
    {
        [SerializeField] private RectTransform fillBar;
        [SerializeField] private Button btn;

        [SerializeField] private SoundID loadSound;
        [SerializeField] private SoundID successSound;
        
        private float _curGage;
        
        protected bool isHold = false;
        
        private Tween _tween;

        private bool _isPlaying;
        
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
                if (!_isPlaying)
                {
                    loadSound.Play();
                    _isPlaying = true;
                }
            }
            else
            {
                if (_isPlaying)
                {
                    BroAudio.Stop(loadSound);
                    _isPlaying = false;
                }

                _curGage = .0f;
            }
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
            
            if (fillBar.localScale.x >= 1f && _isPlaying)
            {
                BroAudio.Stop(loadSound);
                isHold = false;
                _curGage = .0f;
                HandleBtnClick();
                _isPlaying = false;
                successSound.Play();
            }
        }

        private void OnDestroy()
        {
            if (_tween.IsActive()) _tween.Kill();
        }

        protected abstract void HandleBtnClick();

        public virtual void HandleDown() => isHold = true;

        public virtual void HandleUp() => isHold = false;
    }
}