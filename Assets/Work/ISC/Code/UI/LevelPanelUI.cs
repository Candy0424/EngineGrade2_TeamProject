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
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<OpenBookUIEvent>.OnEvent -= HandleOpenUI;
        }

        private void HandleOpenUI(OpenBookUIEvent evt)
        {
            _stageInfo = evt.StageInfo;
            InfoUpdate();
        }

        public void EnableAnimation()
        {
            Sequence seq = DOTween.Sequence()
                .Append(transform.DOScale(1.2f, 1f).SetEase(Ease.InSine))
                .Append(transform.DOScale(0.9f, 1f).SetEase(Ease.OutSine))
                .Append(transform.DOScale(1.1f, 1f).SetEase(Ease.OutSine));
            seq.Play();
        }
        
        private void OnEnable()
        {
            EnableAnimation();
        }

        private void InfoUpdate()
        {
            Debug.Log(_stageInfo);
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
        }
    }
}