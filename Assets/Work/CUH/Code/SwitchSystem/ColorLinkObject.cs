using System;
using UnityEngine;

namespace Work.CUH.Code.SwitchSystem
{
    public class ColorLinkObject : MonoBehaviour
    {
        private Material _renderMaterial;

        private void Awake()
        {
            _renderMaterial = GetComponent<Renderer>().material;
            _renderMaterial = new Material(_renderMaterial);
            GetComponent<Renderer>().material = _renderMaterial;
        }

        public void SetLinkColor(Color color)
            => _renderMaterial.color = color;

        public Color GetLinkColor() => _renderMaterial.color;
    }
}