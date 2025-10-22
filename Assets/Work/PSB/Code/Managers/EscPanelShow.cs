using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.PSB.Code.Managers
{
    public class EscPanelShow : MonoBehaviour
    {
        [SerializeField] private GameObject escPanel;
        [SerializeField] private Texture2D texture;

        private bool isOpen = false;

        private void Start()
        {
            Time.timeScale = 1;
            escPanel.SetActive(false);
            LockCursor();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                isOpen = !isOpen;

            if (isOpen)
            {
                Time.timeScale = 0;
                escPanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                escPanel.SetActive(false);
                Cursor.visible = true;
                Invoke(nameof(LockCursor), 0.05f);
            }
        }

        private void LockCursor()
        {
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.ForceSoftware);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
        
        
    }
}