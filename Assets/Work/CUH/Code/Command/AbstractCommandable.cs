using System;
using UnityEngine;

namespace Work.CUH.Code.Commands
{
    /// <summary>
    /// 커멘드를 수핼할 수 있는 클래스들(수신자)
    /// 커멘드가 이거 해라하고 지시하면 그것의 상세한 동작을 맡는다.
    /// </summary>
    public class AbstractCommandable : MonoBehaviour
    {
        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            
        }
        
        protected virtual void OnDestroy()
        {
        }
    }
}