using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    ///     A component which represents a child relation to its parent.
    ///     Not transforms ! Relation only
    /// </summary>
    public class Child : MonoBehaviour {
        
        [Tooltip("The relational parent of this gameobject")] 
        [SerializeField] private GameObject parent;
        [SerializeField] private UnityEvent<GameObject> onSetEvent;
        
        [SerializeField] private Action<GameObject> _onSet = gm => { };

        /// <summary>
        ///     The relational parent of this gameobject
        /// </summary>
        public GameObject Parent {
            get => parent;
            set {
                parent = value;
                OnSetEvent?.Invoke(parent);
                OnSet(parent);
            }
        }

        /// <summary>
        ///     A unity event getting called once <see cref="Parent" /> getting set
        /// </summary>
        public UnityEvent<GameObject> OnSetEvent {
            get => onSetEvent;
            set => onSetEvent = value;
        }

        /// <summary>
        ///     A action getting called once <see cref="Parent" /> getting set
        /// </summary>
        public Action<GameObject> OnSet {
            get => _onSet;
            set => _onSet = value;
        }
    }
}