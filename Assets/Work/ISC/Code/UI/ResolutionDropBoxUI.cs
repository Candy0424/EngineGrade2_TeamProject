using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class ResolutionDropBoxUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;

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
        }

        public void SetResolution()
        {
            Resolution res = _resolutions[dropdown.value];
            Screen.SetResolution(res.width, res.height, false);
        }
    }
}