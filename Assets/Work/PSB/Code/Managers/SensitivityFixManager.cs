using System;
using Chuh007Lib.Dependencies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.PSB.Code.LibraryPlayers;

namespace Work.PSB.Code.Managers
{
    public class SensitivityFixManager : MonoBehaviour
    {
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private TextMeshProUGUI sensitivityText;
        
        [Inject] private LibraryMovement player;

        private void Start()
        {
            LoadVolume();
            sensitivitySlider.onValueChanged.AddListener(delegate { Sensitivity(); });
        }

        public void Sensitivity()
        {
            player.MouseSensitivity = sensitivitySlider.value;
            PlayerPrefs.SetFloat("sensitivity", player.MouseSensitivity);
            sensitivityText.text = $"감도 : {(player.MouseSensitivity).ToString("0.0")}";
        }
        
        private void LoadVolume()
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", 1f);

            Sensitivity();
        }
        
    }
}