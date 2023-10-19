using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    ///     A component which acts as a Parent and stores multiple child entities or gameobjects.
    ///     Not for transform, for relational purposes.
    /// </summary>
    public class Parent : MonoBehaviour {
        
        [Tooltip("The childs this parent has a relation to")] 
        [SerializeField] private List<GameObject> childs = new List<GameObject>();

        [SerializeField] private UnityEvent<GameObject> onChildAddEvent;
        [SerializeField] private UnityEvent<GameObject> onChildRemoveEvent;

        [SerializeField] private Action<GameObject> _onChildAdd;
        [SerializeField] private Action<GameObject> _onChildRemove;

        /// <summary>
        ///     Adds a Child
        /// </summary>
        /// <param name="child">The child we wanna add to the relation</param>
        public void AddChild(GameObject child) {
            Childs.Add(child);
            OnChildAddEvent?.Invoke(child);
            OnChildAdd(child);
        }

        /// <summary>
        ///     Adds a Child
        /// </summary>
        /// <param name="child">The child we wanna add to the relation</param>
        public void RemoveChild(GameObject child) {
            Childs.Remove(child);
            OnChildRemoveEvent?.Invoke(child);
            OnChildRemove(child);
        }

        /// <summary>
        ///     A list of childs this parent has a relation to
        /// </summary>
        public List<GameObject> Childs {
            get => childs;
            set => childs = value;
        }

        /// <summary>
        ///     A unity event gettin triggered once a new child was added
        /// </summary>
        public UnityEvent<GameObject> OnChildAddEvent {
            get => onChildAddEvent;
            set => onChildAddEvent = value;
        }

        /// <summary>
        ///     A unity event gettin triggered once a child was removed
        /// </summary>
        public UnityEvent<GameObject> OnChildRemoveEvent {
            get => onChildRemoveEvent;
            set => onChildRemoveEvent = value;
        }

        /// <summary>
        ///     A callback gettin triggered once a new child was added
        /// </summary>
        public Action<GameObject> OnChildAdd {
            get => _onChildAdd;
            set => _onChildAdd = value;
        }

        /// <summary>
        ///     A callback gettin triggered once a child was removed
        /// </summary>
        public Action<GameObject> OnChildRemove {
            get => _onChildRemove;
            set => _onChildRemove = value;
        }
    }
}