using System;
using UnityEngine;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.SwitchSystem
{
    public class Lever : MonoBehaviour, ISwitch, ICommandable
    {
        private static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Material onMaterial;
        [SerializeField] private Material offMaterial;
        [SerializeField] private Animator animator;
        
        [Header("Target")]
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
        
        private bool _isActive;
        
        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                if (_isActive)
                {
                    foreach (var render in renderers)
                    {
                        render.material = onMaterial;
                    }
                    animator.SetBool(Open, true);
                    activatable.Activate();
                }
                else
                {
                    foreach (var render in renderers)
                    {
                        render.material = offMaterial;
                    }
                    animator.SetBool(Open, false);
                    activatable.Deactivate();
                }
            }
        }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            activatable = operateObject.GetComponent<IActivatable>();
        }

        public IActivatable activatable { get; private set; }
        
        public void ToggleSwitch()
        {
            IsActive = !IsActive;
        }

        public void UndoSwitch()
        {
            IsActive = !IsActive;
        }
    }
}