using UnityEngine;
using Work.CUH.Chuh007Lib.EventBus;

namespace Work.PSB.Code.Managers
{
    public class CursorToggleEvent : IEvent
    {
        public readonly bool ShouldShow;
        public readonly Texture2D CursorTexture;

        public CursorToggleEvent(bool shouldShow, Texture2D cursorTexture = null)
        {
            ShouldShow = shouldShow;
            CursorTexture = cursorTexture;
        }
        
    }
}