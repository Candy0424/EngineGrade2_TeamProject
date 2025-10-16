using System;
using UnityEngine;
using Work.CIW.Code.Grid;

namespace Work.CUH.Code.SwitchSystem
{
    public class Door : GridObjectBase, IActivatable
    {
        
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void Activate()
        {
            _collider.enabled = false;
        }

        public void Deactivate()
        {
            _collider.enabled = true;
        }
        
        #region Grid

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied()
        {
            throw new NotImplementedException();
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}