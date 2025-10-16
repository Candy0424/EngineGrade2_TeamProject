using System;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.SwitchSystem
{
    public class Lever : GridObjectBase, ICommandable, ISwitch
    {
        public bool isActive { get; private set; }
        
        public event Action<bool> OnSwitchChanged;
        
        public void SwitchOn()
        {
            isActive = !isActive;
        }

        public void SwitchOff()
        {
            isActive = !isActive;
        }

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
        }
    }
}