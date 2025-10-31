using Ami.BroAudio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Work.ISC.Code.UI
{
    public abstract class AbstractScaleButtonUI : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private SoundID clickSound;
        
        private void Awake()
        {
            Initialize();
        }
        
        protected virtual void Initialize()
        {
            btn.onClick.AddListener(HandleClick);
        }

        protected virtual void HandleClick()
        {
            clickSound.Play();
        }
        
        public void HandleEnter() => transform.DOScale(1.2f, 0.2f).SetEase(Ease.Linear).SetAutoKill(true);

        public void HandleExit() => transform.DOScale(1f, 0.2f).SetEase(Ease.Linear).SetAutoKill(true);
    }
}