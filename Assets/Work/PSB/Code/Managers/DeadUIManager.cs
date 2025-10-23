using UnityEngine;
using UnityEngine.SceneManagement;

namespace Work.PSB.Code.Managers
{
    public class DeadUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject deadPanel;

        public void OpenDeadPanel()
        {
            deadPanel.SetActive(true);
        }

        public void RetryGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            SceneManager.LoadScene("BookScene");
        }
        
    }
}