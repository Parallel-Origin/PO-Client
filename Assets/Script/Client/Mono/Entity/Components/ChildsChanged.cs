using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    /// A component that keeps track of the childs and fire events once a child was added or removed.
    /// </summary>
    public class ChildsChanged : MonoBehaviour {

        [SerializeField] private UnityEvent<GameObject> firstEvent;
        [SerializeField] private UnityEvent<GameObject> childAddedEvent;
        [SerializeField] private UnityEvent<GameObject> childRemovedEvent;
        [SerializeField] private UnityEvent emptyEvent;
        
        [SerializeField] private Action<GameObject> _first = o => {};
        [SerializeField] private Action<GameObject> _childAdded = o => {};
        [SerializeField] private Action<GameObject> _childRemoved = o => {};
        [SerializeField] private Action _empty = () => {};
        
        private void OnTransformChildrenChanged() {
            
            var updatedChilds = new List<Transform>();
            foreach(Transform tra in transform) updatedChilds.Add(tra);

            var addedChilds = updatedChilds.Except(Childs).ToList();
            var removedChilds = Childs.Except(updatedChilds).ToList();

            if (Childs.Count <= 0) {
                FirstEvent?.Invoke(addedChilds[0].gameObject);
                First(addedChilds[0].gameObject);
            }
            
            foreach (var child in addedChilds) {
                ChildAddedEvent?.Invoke(child.gameObject);
                ChildAdded(child.gameObject);
            }

            foreach (var child in removedChilds) {
                ChildRemovedEvent?.Invoke(child.gameObject);
                ChildRemoved(child.gameObject);
            }

            if (Childs.Count >= 0 && updatedChilds.Count <= 0) {
                EmptyEvent?.Invoke();
                Empty();
            }

            Childs = updatedChilds;
        }

        /// <summary>
        /// A list of childs being tracked for determining what childs where added and removed
        /// </summary>
        public IList<Transform> Childs { get; set; } = new List<Transform>();

        /// <summary>
        /// Child was added event
        /// </summary>
        public UnityEvent<GameObject> ChildAddedEvent {
            get => childAddedEvent;
            set => childAddedEvent = value;
        }

        /// <summary>
        /// Child was removed event
        /// </summary>
        public UnityEvent<GameObject> ChildRemovedEvent {
            get => childRemovedEvent;
            set => childRemovedEvent = value;
        }

        /// <summary>
        /// A event getting fired when the first child was inserted
        /// </summary>
        public UnityEvent<GameObject> FirstEvent {
            get => firstEvent;
            set => firstEvent = value;
        }

        /// <summary>
        /// Event getting fired when the last child was removed
        /// </summary>
        public UnityEvent EmptyEvent {
            get => emptyEvent;
            set => emptyEvent = value;
        }

        /// <summary>
        /// Child was added event
        /// </summary>
        public Action<GameObject> ChildAdded {
            get => _childAdded;
            set => _childAdded = value;
        }

        /// <summary>
        /// Child was removed event
        /// </summary>
        public Action<GameObject> ChildRemoved {
            get => _childRemoved;
            set => _childRemoved = value;
        }

        /// <summary>
        /// A event getting fired when the first child was inserted
        /// </summary>
        public Action<GameObject> First {
            get => _first;
            set => _first = value;
        }
        
        /// <summary>
        /// Event getting fired when the last child was removed
        /// </summary>
        public Action Empty {
            get => _empty;
            set => _empty = value;
        }
    }
}