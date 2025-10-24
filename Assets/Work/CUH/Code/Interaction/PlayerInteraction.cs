using System;
using Blade.Entities;
using UnityEngine;

namespace Work.CUH.Code.Interaction
{
    public class PlayerInteraction : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private LayerMask whatisTarget;
        [SerializeField] private float interactionRange = 2f;
        
        private Entity _entity;
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void CheckInteraction()
        {
            Physics.Raycast(transform.position, transform.forward * interactionRange, out RaycastHit hit, whatisTarget);

            if (hit.collider && hit.collider.TryGetComponent(out IInteraction interaction))
            {
                interaction.Interact();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * interactionRange);
        }
    }
}