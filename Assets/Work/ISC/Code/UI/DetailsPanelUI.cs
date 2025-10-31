using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.ISC.Code.UI
{
    public class DetailsPanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private Toggle toggle;

        private List<Resolution> _resolutions;
        private List<string> _dropdownMenus;

        private const string PREF_RESOLUTION_INDEX = "ResolutionIndex";
        private const string PREF_FULLSCREEN = "Fullscreen";

        private void Awake()
        {
            _resolutions = new List<Resolution>();
            _dropdownMenus = new List<string>();
            Initialize();
        }

        private void Initialize()
        {
            dropdown.ClearOptions();

            foreach (var resolution in Screen.resolutions)
            {
                if ((resolution.width == 1280 && resolution.height == 720) ||
                    (resolution.width > 1600 && resolution.height > 1000))
                {
                    RefreshRate rate = resolution.refreshRateRatio;
                    uint refreshRate = rate.numerator / rate.denominator;
                    string str = $"{resolution.width}x{resolution.height} {refreshRate}Hz";
                    _dropdownMenus.Add(str);
                    _resolutions.Add(resolution);
                }
            }

            dropdown.AddOptions(_dropdownMenus);
            
            int savedResolutionIndex = PlayerPrefs.GetInt(PREF_RESOLUTION_INDEX, 0);
            bool savedFullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;

            dropdown.value = Mathf.Clamp(savedResolutionIndex, 0, _dropdownMenus.Count - 1);
            toggle.isOn = savedFullscreen;

            SetResolution();
        }

        public void SetResolution()
        {
            int index = dropdown.value;

            Resolution res = _resolutions[index];
            Screen.SetResolution(res.width, res.height, toggle.isOn);
            
            PlayerPrefs.SetInt(PREF_RESOLUTION_INDEX, index);
            PlayerPrefs.Save();
        }

        public void SetFullScreen(bool isFull)
        {
            Screen.fullScreen = isFull;
            
            PlayerPrefs.SetInt(PREF_FULLSCREEN, isFull ? 1 : 0);
            PlayerPrefs.Save();
        }
        
    }
}
