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
        /// <summary>
        /// 해당 객체가 수행 가능한 커멘드들입니다.
        /// </summary>
        [field: SerializeField] public BaseCommandSO[] AvailableCommands { get; private set; }
        // 만약 MoveCommand와 DestroyCommand가 있다면 이 놈은 움직일 수 있고 부술 수 있는 겁니다.
        
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