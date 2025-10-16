using System;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.SwitchSystem
{
    public class Lever : GridObjectBase, ICommandable, ISwitch
    {
        
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                if (_isActive) activatable.Activate();
                else activatable.Deactivate();
            }
        }
        
        public IActivatable activatable { get; private set; }
        
        private bool _isActive;
        
        public void ToggleSwitch()
        {
            IsActive = !IsActive;
        }

        public void UndoSwitch()
        {
            IsActive = !IsActive;
        }
        
        #region Grid
        
        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
        }
        
        #endregion

    }
}