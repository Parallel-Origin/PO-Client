using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity {
    
    /// <summary>
    ///     A script for the <see cref="MonoBehaviour" /> World which references to a <see cref="Unity.Entities.Entity" /> inside the Dots world
    /// </summary>
    public class EcsEntity : MonoBehaviour {
        
        [SerializeField] private long index;
        [SerializeField] private Unity.Entities.Entity _entity;
        
        [SerializeField] private UnityEvent<Unity.Entities.Entity> onReferenceEvent;
        [SerializeField] private Action<Unity.Entities.Entity> _onReference = assignedEntity => { };

        /// <summary>
        ///     The entity index/id
        /// </summary>
        public long Index {
            get => index;
            set => index = value;
        }

        /// <summary>
        ///     The reference to the <see cref="_entity" /> this <see cref="GameObject" /> is represented by inside the ecs.
        ///     Used as a bridge between Monos and Dots.
        /// </summary>
        public Unity.Entities.Entity EntityReference {
            get => _entity;
            set {
                _entity = value;
                OnReferenceEvent?.Invoke(_entity);
                OnReference?.Invoke(_entity);
            }
        }

        /// <summary>
        ///     A <see cref="UnityEvent{T0}" /> for the inspector getting triggered once the local <see cref="EntityReference" /> was assigned.
        /// </summary>
        public UnityEvent<Unity.Entities.Entity> OnReferenceEvent {
            get => onReferenceEvent;
            set => onReferenceEvent = value;
        }

        /// <summary>
        ///     A callback getting triggered once the local <see cref="EntityReference" /> was assigned, passing the assigned entity.
        /// </summary>
        public Action<Unity.Entities.Entity> OnReference {
            get => _onReference;
            set => _onReference = value;
        }
    }
}