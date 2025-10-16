using System;

namespace Work.CUH.Code.SwitchSystem
{
    public interface ISwitch
    {
        bool isActive { get; }
        event Action<bool> OnSwitchChanged;
        
        void SwitchOn();
        void SwitchOff();
    }
}