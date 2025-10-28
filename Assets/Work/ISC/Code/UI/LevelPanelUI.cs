using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;
using Work.ISC.Code.SO;

namespace Work.ISC.Code.UI
{
    public class LevelPanelUI : MonoBehaviour
    {
        [Header("Prefab Settings")]
        [SerializeField] private GameObject colorStarPrefab;
        [SerializeField] private GameObject nonColorStarPrefab;
        [SerializeField] private Transform levelTrm;
        
        [Header("Component Settings")]
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI sub;
        [SerializeField] private TextMeshProUGUI first;
        [SerializeField] private TextMeshProUGUI second;
        [SerializeField] private TextMeshProUGUI third;
        
        [Header("Level Settings")]
        [SerializeField] private int maxLevel;
        
        private List<GameObject> _stars;

        private StageInfoSO _stageInfo;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _stars = new List<GameObject>();

            Bus<OpenBookUIEvent>.OnEvent += HandleOpenUI;
            Bus<CloseBookUIEvent>.OnEvent += HandleCloseUI;
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<OpenBookUIEvent>.OnEvent -= HandleOpenUI;
            Bus<CloseBookUIEvent>.OnEvent -= HandleCloseUI;
        }

        private void HandleCloseUI(CloseBookUIEvent evt)
        {
            DisableAnimation();
        }

        private void HandleOpenUI(OpenBookUIEvent evt)
        {
            _stageInfo = evt.StageInfo;
            InfoUpdate();
        }

        private void EnableAnimation()
        {
            Sequence seq = DOTween.Sequence()
                .SetUpdate(UpdateType.Normal, true)
                .Append(transform.DOScale(1.2f, 0.2f).SetEase(Ease.InSine))
                .Append(transform.DOScale(0.9f, 0.2f).SetEase(Ease.OutSine))
                .Append(transform.DOScale(1f, 0.2f).SetEase(Ease.InSine))
                .SetAutoKill(false)
                .Pause();
            seq.Play();
        }
        
        private void OnEnable()
        {
            EnableAnimation();
        }

        private void OnDisable()
        {
            DisableAnimation();
        }

        private void DisableAnimation()
        {
            Sequence seq = DOTween.Sequence()
                .SetUpdate(UpdateType.Normal, true)
                .Append(transform.DOScale(0.1f, 0.2f).SetEase(Ease.OutSine))
                .SetAutoKill(false)
                .Pause();
            
            seq.OnComplete(() => gameObject.SetActive(false));
            seq.Play();
        }

        private void InfoUpdate()
        {
            if (_stars.Count > 0)
            {
                foreach (GameObject star in _stars)
                {
                    Destroy(star);
                }
                _stars.Clear();
            }
            
            for (int i = 1; i <= maxLevel; i++)
            {
                GameObject o = Instantiate(i > _stageInfo.level ? nonColorStarPrefab : colorStarPrefab, levelTrm);

                Debug.Assert(o != null, $"올바르지 않은 레벨 : {i}");
                
                _stars.Add(o);
            }

            image.sprite = _stageInfo.stageImg;
            title.SetText(_stageInfo.stageName);
            sub.SetText(_stageInfo.description);
            first.SetText(_stageInfo.firstStd);
            second.SetText(_stageInfo.secondStd);
            third.SetText(_stageInfo.thirdStd);
        }
    }
}