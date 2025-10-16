using System;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CIW.Code.Grid;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.SwitchSystem
{
    public class Lever : GridObjectBase, ICommandable, ISwitch
    {
        [SerializeField] private GameObject operateObject;
        
        public GameObject activeObject
        {
            get => operateObject;
            private set
            {
                if (operateObject.TryGetComponent(out IActivatable activate))
                {
                    operateObject = value;
                    activatable = activate;
                }
            }
        }
        
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                if (!_isActive) activatable.Activate();
                else activatable.Deactivate();
            }
        }
        
        public IActivatable activatable { get; private set; }
        
        private bool _isActive;
        
        [ContextMenu("Activate")]
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (operateObject.TryGetComponent(out IActivatable activate))
            {
                activeObject = operateObject;
            }
            else
            {
                operateObject = null;
                Debug.LogError("This Object is not ActivateObject");
            }
        }
#endif
    }
}