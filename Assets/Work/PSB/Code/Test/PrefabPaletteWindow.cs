using UnityEditor;
using UnityEngine;

namespace Work.PSB.Code.Test
{
    public class PrefabPaletteWindow : EditorWindow
    {
        private static PrefabPaletteData paletteData;
        private Vector3 spawnPosition = Vector3.zero;
        private int selectedIndex = -1;

        [MenuItem("Tools/Prefab Palette")]
        public static void ShowWindow()
        {
            GetWindow<PrefabPaletteWindow>("Prefab Palette");
        }

        private void OnEnable()
        {
            if (paletteData == null)
                LoadOrCreatePaletteData();
        }

        private void OnGUI()
        {
            GUILayout.Label("🎨 Prefab Palette (3D)", EditorStyles.boldLabel);

            spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", spawnPosition);
            EditorGUILayout.Space();

            if (paletteData != null && paletteData.prefabs != null)
            {
                for (int i = 0; i < paletteData.prefabs.Length; i++)
                {
                    if (paletteData.prefabs[i] == null) continue;
                    
                    Color defaultColor = GUI.color;
                    if (i == selectedIndex)
                    {
                        GUI.color = Color.green;
                    }

                    if (GUILayout.Button(paletteData.prefabs[i].name, GUILayout.Height(30)))
                    {
                        selectedIndex = i;
                    }

                    GUI.color = defaultColor;
                }
            }

            EditorGUILayout.Space();

            if (selectedIndex >= 0 && selectedIndex < paletteData.prefabs.Length)
            {
                if (GUILayout.Button("Spawn Selected Prefab", GUILayout.Height(40)))
                {
                    SpawnPrefab(paletteData.prefabs[selectedIndex]);
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Palette Asset"))
            {
                Selection.activeObject = paletteData;
            }
        }

        private void SpawnPrefab(GameObject prefab)
        {
            if (prefab == null) return;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = spawnPosition;

            Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
            Selection.activeGameObject = instance;
        }

        private void LoadOrCreatePaletteData()
        {
            string[] guids = AssetDatabase.FindAssets("t:PrefabPaletteData");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                paletteData = AssetDatabase.LoadAssetAtPath<PrefabPaletteData>(path);
            }
            else
            {
                paletteData = ScriptableObject.CreateInstance<PrefabPaletteData>();
                AssetDatabase.CreateAsset(paletteData, "Assets/Editor/PrefabPaletteData.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("PrefabPaletteData.asset 자동 생성됨!");
            }
        }
        
    }
}