using Script.Client.Mono.Entity.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Entity {
    
     /// <summary>
    ///     A interface for a entity which represents either a UI-Element or gameobject.
    ///     Contains methods for controlling events based upon the enable state.
    /// </summary>
    public interface IEntity {

        /// <summary>
        ///     Unity - Event, executed registered actions.
        /// </summary>
        void OnEnable();

        /// <summary>
        ///     Unity - Event, executed registered actions.
        /// </summary>
        void OnDisable();

        /// <summary>
        ///     Makes the field hide.
        /// </summary>
        void Hide();

        /// <summary>
        ///     Makes the field show.
        /// </summary>
        void Show();

        /// <summary>
        ///     The Element-Category of this UI-Element.
        ///     Used by managers to group them for easier acess.
        /// </summary>
        string ElementCategory { get; set; }

        /// <summary>
        ///     Determines if this object is enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        ///     Sets if the callback should get fired once on enabled, or everytime
        /// </summary>
        bool OnceOnEnabled { get; set; }

        /// <summary>
        ///     Sets if the callback should get fired once on disabled, or everytime
        /// </summary>
        bool OnceOnDisabled { get; set; }

        /// <summary>
        ///     The callback getting fired once we enable this object
        /// </summary>
        UnityEvent OnEnableCallback { get; set; }

        /// <summary>
        ///     The callback getting fired once we disable this object
        /// </summary>
        UnityEvent OnDisableCallback { get; set; }

        /// <summary>
        ///     The gameobject this element is attached to
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        ///     Reference to a component which acts as a parent that stores references to its relational childs
        /// </summary>
        Parent AsParent { get; set; }

        /// <summary>
        ///     The owner, of this UI-Element.
        ///     Not the gameobject-parent, instead the "owner" like a popup, or button or whatever... the "real" owner.
        /// </summary>
        Child AsChild { get; set; }
    }
    
    /// <summary>
    ///     A component which represents a Element, either UI or ingame with its most nessecary actions and callbacks.
    /// </summary>
    [RequireComponent(typeof(Parent))]
    [RequireComponent(typeof(Child))]
    public class Entity : MonoBehaviour, IEntity {
        
        [SerializeField] private string category = "UI";
        [SerializeField] private bool enabled;
        [SerializeField] private bool onceOnEnabled;
        [SerializeField] private bool onceOnDisabled;

        [SerializeField] private UnityEvent onEnable;
        [SerializeField] private UnityEvent onDisable;
        
        private Child _asChild;
        private Parent _asParent;

        private void Awake() {
            _asParent = GetComponent<Parent>();
            _asChild = GetComponent<Child>();
        }

        /////////////////////////////
        /// Main methods
        /////////////////////////////


        /// <summary>
        ///     Unity - Event, executed registered actions.
        /// </summary>
        public void OnEnable() {
            if (onEnable != null)
                onEnable.Invoke();

            if (onceOnEnabled)
                onEnable = null;

            enabled = true;
        }


        /// <summary>
        ///     Unity - Event, executed registered actions.
        /// </summary>
        public void OnDisable() {
            if (onDisable != null)
                onDisable.Invoke();

            if (onceOnDisabled)
                onDisable = null;

            enabled = false;
        }

        /// <summary>
        ///     Makes the field hide.
        /// </summary>
        public virtual void Hide() { gameObject.SetActive(false); }

        /// <summary>
        ///     Makes the field show.
        /// </summary>
        public virtual void Show() { gameObject.SetActive(true); }


        /////////////////////////////
        /// Properties methods
        /////////////////////////////


        /// <summary>
        ///     Reference to a component which acts as a parent that stores references to its relational childs
        /// </summary>
        public Parent AsParent {
            get => _asParent;
            set => _asParent = value;
        }

        /// <summary>
        ///     The owner, of this UI-Element.
        ///     Not the gameobject-parent, instead the "owner" like a popup, or button or whatever... the "real" owner.
        /// </summary>
        public Child AsChild {
            get => _asChild;
            set => _asChild = value;
        }

        /// <summary>
        ///     The gameobject this element is attached to
        /// </summary>
        public GameObject GameObject => gameObject;

        /// <summary>
        ///     The Category, this element belongs to
        /// </summary>
        public string ElementCategory {
            get => category;
            set => category = value;
        }

        /// <summary>
        ///     Determines if this object is enabled
        /// </summary>
        public bool Enabled {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>
        ///     Sets if the callback should get fired once on enabled, or everytime
        /// </summary>
        public bool OnceOnEnabled {
            get => onceOnEnabled;
            set => onceOnEnabled = value;
        }

        /// <summary>
        ///     Sets if the callback should get fired once on disabled, or everytime
        /// </summary>
        public bool OnceOnDisabled {
            get => onceOnDisabled;
            set => onceOnDisabled = value;
        }

        /// <summary>
        ///     The callback getting fired once we enable this object
        /// </summary>
        public UnityEvent OnEnableCallback {
            get => onEnable;
            set => onEnable = value;
        }

        /// <summary>
        ///     The callback getting fired once we disable this object
        /// </summary>
        public UnityEvent OnDisableCallback {
            get => onDisable;
            set => onDisable = value;
        }
    }
}