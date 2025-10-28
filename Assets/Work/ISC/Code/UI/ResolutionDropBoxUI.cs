using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Work.ISC.Code.UI
{
    public class ResolutionDropBoxUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;

        private List<Resolution> _resolutions;
        
        private void Awake()
        {
            _resolutions = new List<Resolution>();
            Initialize();
        }

        private void Initialize()
        {

            foreach (var resolution in Screen.resolutions)
            {
                if ((resolution.width == 1280 && resolution.height == 720) || (resolution.width > 1600 && resolution.height > 1000))
                    _resolutions.Add(resolution);
            }
            
            Debug.Log(Screen.resolutions.Length);
            foreach (var resolution in _resolutions)
            {
                RefreshRate rate = resolution.refreshRateRatio;
                uint refreshRate = rate.numerator / rate.denominator;
                Debug.Log(resolution.width + " x " + resolution.height + " " + refreshRate + "Hz");
            }
        }
    }
}