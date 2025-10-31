using Chuh007Lib.ObjectPool.Runtime;
using UnityEngine;

namespace Work.ISC.Code.Effects
{
    public class PoolingEffect : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolingItemSO PoolingType { get; private set; }
        [SerializeField] private GameObject effectObject;

        private Pool _myPool;
        private IPlayableVFX _playableVFX;
        
        public GameObject GameObject => gameObject;
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>();
        }

        public void ResetItem()
        {
            
        }

        private void OnValidate()
        {
            if (effectObject == null) return;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>();
            if (_playableVFX == null)
            {
                effectObject = null;
            }
        }

        public void PlayVFX(Vector3 hitPoint)
        {
            _playableVFX.PlayVfx(hitPoint);
        }
    }
}