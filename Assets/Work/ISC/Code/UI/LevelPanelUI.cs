using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private StageInfoSO stageInfo;
        [SerializeField] private int maxLevel;
        
        private List<GameObject> _stars;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            image = GetComponentInChildren<Image>();
            title = GetComponentInChildren<TextMeshProUGUI>();
            sub = GetComponentInChildren<TextMeshProUGUI>();
            _stars = new List<GameObject>();
            InfoUpdate();
        }

        public void Enable()
        {
            
        }
        
        private void OnEnable()
        {
            // InfoUpdate();
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
                GameObject o = Instantiate(i > stageInfo.level ? nonColorStarPrefab : colorStarPrefab, levelTrm);

                Debug.Assert(o != null, $"올바르지 않은 레벨 : {i}");
                
                _stars.Add(o);
            }

            image.sprite = stageInfo.stageImg;
            title.SetText(stageInfo.stageName);
            sub.SetText(stageInfo.description);
        }
    }
}