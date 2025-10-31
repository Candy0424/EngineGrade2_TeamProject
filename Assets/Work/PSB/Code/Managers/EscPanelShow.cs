using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Work.CUH.Chuh007Lib.EventBus;
using Work.PSB.Code.Managers; // CursorManager를 사용하기 위해 추가

namespace Work.PSB.Code.Managers
{
    public class EscPanelShow : MonoBehaviour
    {
        [SerializeField] private GameObject escPanel;
        [SerializeField] private Texture2D texture;

        private bool isOpen = false; // ESC 패널 상태
        private Texture2D cachedCursor;
        private bool isCursorForcedOpen = false; // 외부 UI(예: StageSelectionUI)가 커서를 강제 오픈했는지 여부
        
        public void SetOpen(bool open) => isOpen = open;
        
        private void Start()
        {
            Time.timeScale = 1;
            escPanel.SetActive(false);

            float scaleFactor = 2f;
            int newWidth = Mathf.RoundToInt(texture.width * scaleFactor);
            int newHeight = Mathf.RoundToInt(texture.height * scaleFactor);

            cachedCursor = ResizeTexture(texture, newWidth, newHeight);

            SetCursorLockedState(true);
            
            Bus<CursorToggleEvent>.OnEvent += HandleCursorToggleRequest;
        }

        private void OnDestroy()
        {
            if (cachedCursor != null)
                Destroy(cachedCursor);
            
            SetCursorLockedState(false);
            Bus<CursorToggleEvent>.OnEvent -= HandleCursorToggleRequest;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame && !isCursorForcedOpen)
            {
                isOpen = !isOpen;
            }

            if (isOpen)
            {
                Time.timeScale = 0;
                escPanel.SetActive(true);
                SetCursorLockedState(false);
            }
            else if (!isCursorForcedOpen)
            {
                Time.timeScale = 1;
                escPanel.SetActive(false);
                SetCursorLockedState(true);
            }
        }

        private void HandleCursorToggleRequest(CursorToggleEvent evt)
        {
            isCursorForcedOpen = evt.ShouldShow;

            if (evt.ShouldShow)
            {
                Time.timeScale = 0;
                SetCursorLockedState(false, evt.CursorTexture);
            }
            else if (!isOpen)
            {
                Time.timeScale = 1;
                SetCursorLockedState(true);
            }
        }

        private void SetCursorLockedState(bool locked, Texture2D textureOverride = null)
        {
            if (locked)
            {
                Cursor.SetCursor(cachedCursor, Vector2.zero, CursorMode.ForceSoftware);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
            }
            else
            {
                Texture2D cursorToUse = textureOverride != null ? textureOverride : null;
                Cursor.SetCursor(cursorToUse, Vector2.zero, CursorMode.Auto);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
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