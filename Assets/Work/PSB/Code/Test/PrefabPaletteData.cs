using UnityEngine;

namespace Work.PSB.Code.Test
{
    [CreateAssetMenu(fileName = "PrefabPaletteData", menuName = "Tools/Prefab Palette Data")]
    public class PrefabPaletteData : ScriptableObject
    {
        public GameObject[] prefabs;
    }
}