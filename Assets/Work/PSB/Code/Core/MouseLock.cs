using UnityEngine;
using UnityEngine.InputSystem;

namespace Work.PSB.Code.Core
{
    public class MouseLock : MonoBehaviour
    {
        [SerializeField] private bool lockMouse;
        
        private void Start()
        {
            if (lockMouse)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                lockMouse = !lockMouse;
            }
            
            if (lockMouse)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        
    }
}