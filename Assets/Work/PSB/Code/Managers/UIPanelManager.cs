using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Work.PSB.Code.Managers
{
    public class UIPanelManager : MonoBehaviour
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button exitBtn;
        
        [SerializeField] private GameObject escPanel;

        private void Start()
        {
            escPanel.SetActive(false);
            
            startBtn.onClick.AddListener(StartButton);
            exitBtn.onClick.AddListener(ExitButton);
            settingBtn.onClick.AddListener(SettingButton);
        }

        public void StartButton()
        {
            SceneManager.LoadScene("LibraryPlayerScene");
        }

        public void SettingButton()
        {
            escPanel.SetActive(true);
        }

        public void SettingCloseButton()
        {
            escPanel.SetActive(false);
        }

        public void ExitButton()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

    }
}