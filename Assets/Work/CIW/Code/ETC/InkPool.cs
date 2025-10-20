using Chuh007Lib.ObjectPool.Runtime;
using UnityEngine;

namespace Work.CIW.Code.ETC
{
    public class InkPool : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolingItemSO PoolingType { get; private set; }

        Pool _pool;

        public GameObject GameObject => gameObject;

        public void ResetItem()
        {

        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }
    }
}