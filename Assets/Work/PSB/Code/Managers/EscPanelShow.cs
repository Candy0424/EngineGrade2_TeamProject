using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.PSB.Code.Managers
{
    public class EscPanelShow : MonoBehaviour
    {
        [SerializeField] private GameObject escPanel;

        private bool isOpen = false;
        
        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isOpen = !isOpen;
            }

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
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Start()
        {
            Time.timeScale = 1;
            escPanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }
}