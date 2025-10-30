using System;
using UnityEngine;
using UnityEngine.UI;

namespace Work.ISC.Code.UI
{
    public abstract class AbstractFillButtonUI : MonoBehaviour
    {
        [SerializeField] private RectTransform fillBar;
        [SerializeField] private Button btn;

        protected bool isHold = false;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        { 
            fillBar.localScale = new Vector3(0, 1, 1);
            
            btn.onClick.AddListener(() => SetHold(true));
        }
        
        protected void SetHold(bool hold) => isHold = hold;

        private void Update()
        {
            
        }
    }
}