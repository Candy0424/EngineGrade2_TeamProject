using UnityEngine;

namespace Work.ISC.Code.Effects
{
    public interface IPlayableVFX
    {
        public string VfxName { get; }
        public void PlayVfx(Vector3 position);
        public void StopVfx();
    }
}