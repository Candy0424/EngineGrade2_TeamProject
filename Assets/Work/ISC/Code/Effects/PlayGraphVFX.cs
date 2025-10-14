using UnityEngine;
using UnityEngine.VFX;
using NotImplementedException = System.NotImplementedException;

namespace Work.ISC.Code.Effects
{
    public class PlayGraphVFX : MonoBehaviour, IPlayableVFX
    {
        [field: SerializeField] public string VfxName { get; private set; }
        [SerializeField] private bool isOnPosition;
        [SerializeField] private VisualEffect[] effects;
        
        public void PlayVfx(Vector3 position)
        {
            if(isOnPosition == false)
                transform.SetPositionAndRotation(position, Quaternion.identity);

            foreach (VisualEffect effect in effects)
                effect.Play();
        }

        public void StopVfx()
        {
            foreach (VisualEffect effect in effects)
                effect.Stop();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(VfxName) == false)
                gameObject.name = VfxName;
        }
    }
}