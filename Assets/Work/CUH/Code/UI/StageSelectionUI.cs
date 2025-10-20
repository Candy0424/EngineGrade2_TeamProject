using System;
using TransitionsPlus;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.UI
{
    public class StageSelectionUI : MonoBehaviour
    {
        [SerializeField] private TransitionAnimator transition;
        [SerializeField] private GameObject stageUI; // 님이 만든 스테이지 UI 넣으셈
        private string _loadSceneName;
        
        private void Awake()
        {
            if(transition == null) transition = GetComponent<TransitionAnimator>();
            Bus<OpenBookUIEvent>.OnEvent += HandleOpenBook;
        }

        private void OnDestroy()
        {
            Bus<OpenBookUIEvent>.OnEvent -= HandleOpenBook;
        }

        private void HandleOpenBook(OpenBookUIEvent evt)
        {
            _loadSceneName = evt.LoadSceneName;
            stageUI.SetActive(true);
        }
        
        [ContextMenu("Test")]
        public void LoadGameScene() // 플레이어가 스테이지 진입 누르면 호출
        {
            transition.gameObject.SetActive(true);
            transition.sceneNameToLoad = _loadSceneName;
        }
    }
}