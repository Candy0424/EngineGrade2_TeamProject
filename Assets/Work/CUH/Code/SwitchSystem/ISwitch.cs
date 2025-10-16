﻿using System;

namespace Work.CUH.Code.SwitchSystem
{
    public interface ISwitch
    {
        bool IsActive { get; }
        
        IActivatable activatable { get; }
        
        void ToggleSwitch();
        void UndoSwitch();
    }
}