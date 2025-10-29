using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
            SetResolution();
            Screen.fullScreen = toggle.isOn;
        }

        public void SetResolution()
        {
            Debug.Log("해상도 변경");
            Resolution res = _resolutions[dropdown.value];
            Screen.SetResolution(res.width, res.height, toggle.isOn);
        }

        public void SetFullScreen(bool isFull)
        {
            Screen.fullScreen = isFull;
        }
    }
}