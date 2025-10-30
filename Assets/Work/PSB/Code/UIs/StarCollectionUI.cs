using TMPro;
using UnityEngine;
using Work.PSB.Code.SaveSystem;

namespace Work.PSB.Code.UIs
{
    public class StarCollectionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshPro starText;
        [SerializeField] private GameObject endObject;

        private void OnEnable()
        {
            if (StageProgressManager.Instance == null)
                return;

            endObject.SetActive(false);
            UpdateStarCount();
        }

        public void UpdateStarCount()
        {
            if (StageProgressManager.Instance == null)
                return;

            int total = StageProgressManager.Instance.GetTotalStars();
            int max = StageProgressManager.Instance.GetMaxStars();
            starText.SetText($"{total} / {max}");
            
            if (endObject != null)
                endObject.SetActive(total >= max);
        }
        
    }
}