using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    ///     A component which stores different callbacks for the typical UI-Lifecycle
    /// </summary>
    public class Callbacks : MonoBehaviour {
        
        [SerializeField] private UnityEvent<Component> onStartEvent;
        [SerializeField] private UnityEvent<Component> onUpdateEvent;
        [SerializeField] private UnityEvent<Component> onDestroyEvent;
        
        [SerializeField] private Action<Component> _onDestroy = component => { };
        [SerializeField] private Action<Component> _onStart = component => { };
        [SerializeField] private Action<Component> _onUpdate = component => { };

        /// <summary>
        ///     Event, called once a component started
        /// </summary>
        public UnityEvent<Component> OnStartEvent {
            get => onStartEvent;
            set => onStartEvent = value;
        }

        /// <summary>
        ///     Event, called once a component updated
        /// </summary>
        public UnityEvent<Component> OnUpdateEvent {
            get => onUpdateEvent;
            set => onUpdateEvent = value;
        }

        /// <summary>
        ///     Event, called once a component/entity was destroyed
        /// </summary>
        public UnityEvent<Component> OnDestroyEvent {
            get => onDestroyEvent;
            set => onDestroyEvent = value;
        }

        /// <summary>
        ///     Callback, triggered once the bypassed component started
        /// </summary>
        public Action<Component> OnStart {
            get => _onStart;
            set => _onStart = value;
        }

        /// <summary>
        ///     Callback, triggered once the bypassed component was updated
        /// </summary>
        public Action<Component> OnUpdate {
            get => _onUpdate;
            set => _onUpdate = value;
        }

        /// <summary>
        ///     Callback, triggered once the bypassed component was destroyed.
        /// </summary>
        public Action<Component> OnDestroy {
            get => _onDestroy;
            set => _onDestroy = value;
        }
    }
}