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
        private Texture2D cachedCursor;

        private void Start()
        {
            Time.timeScale = 1;
            escPanel.SetActive(false);

            float scaleFactor = 2f;
            int newWidth = Mathf.RoundToInt(texture.width * scaleFactor);
            int newHeight = Mathf.RoundToInt(texture.height * scaleFactor);

            cachedCursor = ResizeTexture(texture, newWidth, newHeight);
            LockCursor();
        }

        private void OnDestroy()
        {
            if (cachedCursor != null)
                Destroy(cachedCursor);
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
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
            Cursor.SetCursor(cachedCursor, Vector2.zero, CursorMode.ForceSoftware);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        private Texture2D ResizeTexture(Texture2D source, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(source, rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return result;
        }
        
        
    }
}
