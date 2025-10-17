using System;
using UnityEngine;
using UnityEngine.Serialization;
using Work.CIW.Code.Grid;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.GameEvents;

namespace Work.CUH.Code.SwitchSystem
{
    public class Door : GridObjectBase, IActivatable
    {
        [SerializeField] private GameObject onVisual;
        [SerializeField] private GameObject offVisual;
        
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();

        }

        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;
            GridSystem.Instance.SetObjectInitialPosition(this, CurrentGridPosition);
        }

        public void Activate()
        {
            _collider.enabled = false;
            onVisual.SetActive(false);
            offVisual.SetActive(true);
            GridSystem.Instance.RemoveObjectPosition(this, CurrentGridPosition);
        }

        public void Deactivate()
        {
            _collider.enabled = true;
            offVisual.SetActive(false);
            onVisual.SetActive(true);
            GridSystem.Instance.SetObjectInitialPosition(this, CurrentGridPosition);
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