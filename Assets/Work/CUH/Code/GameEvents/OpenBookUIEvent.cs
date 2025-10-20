using Work.CUH.Chuh007Lib.EventBus;

namespace Work.CUH.Code.GameEvents
{
    public struct OpenBookUIEvent : IEvent
    {
        public string LoadSceneName;
        
        public OpenBookUIEvent(string loadSceneName)
        {
            LoadSceneName = loadSceneName;
        }
        
    }
    
    public struct CloseBookUIEvent : IEvent
    {
        
    }
}