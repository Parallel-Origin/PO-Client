using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    ///     A component which provides a set of actions for the typical UI-Lifecycle
    /// </summary>
    public class Actions : MonoBehaviour {
        
        [SerializeField] private UnityEvent<Component> start;
        [SerializeField] private UnityEvent<Component> update;
        [SerializeField] private UnityEvent<Component> destroy;

        public UnityEvent<Component> Start {
            get => start;
            set => start = value;
        }

        public UnityEvent<Component> Update {
            get => update;
            set => update = value;
        }

        public UnityEvent<Component> Destroy {
            get => destroy;
            set => destroy = value;
        }
    }
}