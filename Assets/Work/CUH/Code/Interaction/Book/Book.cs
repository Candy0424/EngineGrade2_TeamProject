using TransitionsPlus;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.Interaction.Book
{
    public class Book : MonoBehaviour, IInteraction
    {
        [SerializeField] private string loadSceneName;
        
        public void Interact()
        {
            Bus<OpenBookUIEvent>.Raise(new OpenBookUIEvent(loadSceneName));
        }
    }
}