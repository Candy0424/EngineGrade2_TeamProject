using UnityEngine;

namespace Work.ISC.Code.Effects
{
    public class PlayParticleVFX : MonoBehaviour, IPlayableVFX
    {
        [field: SerializeField] public string VfxName { get; private set; }
        [SerializeField] private bool isOnPosition;
        [SerializeField] private ParticleSystem particle;
        
        public void PlayVfx(Vector3 position)
        {
            if (isOnPosition == false)
                transform.SetPositionAndRotation(position, Quaternion.identity);

            particle.Play(true);
        }

        public void StopVfx()
        {
            particle.Stop(true);
        }

        private void OnValidate()
        {
            if(string.IsNullOrEmpty(VfxName) == false)
                gameObject.name = VfxName;
        }
    }
}