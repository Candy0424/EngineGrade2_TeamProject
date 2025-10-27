using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.UI
{
    public class TextShowUI : MonoBehaviour
    {
        [SerializeField] private GameObject textPrefab;

        private void Awake()
        {
            Bus<TextEvent>.OnEvent += HandleTextPrint;
        }

        private void OnDestroy()
        {
            Bus<TextEvent>.OnEvent -= HandleTextPrint;
        }

        private void HandleTextPrint(TextEvent evt)
        {
            GameObject obj = Instantiate(textPrefab, transform);
            obj.GetComponent<TextMeshProUGUI>().text = evt.Text;
            DOVirtual.DelayedCall(0.5f, () => Destroy(obj));
        }
    }
}