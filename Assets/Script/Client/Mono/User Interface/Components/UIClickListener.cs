using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.Client.Mono.User_Interface.Components {
    
    /// <summary>
    ///     Class for all interactables ingame, especially for the UI-Elements & gameobjects.
    ///     Fires a event, once we interact with that object, for example clicking on it
    ///     When the button is set, we will listen for button clicks instead
    /// </summary>
    public class UIClickListener : MonoBehaviour {

        [Tooltip("If this button is set, we will listen for button events instead")] 
        [SerializeField] private Button button;

        [SerializeField] private bool interactable = true;
        [SerializeField] private float coolDownS = 0.1f;

        [SerializeField] private UnityEvent beforeInteract; // Event, executing befre onInteract
        [SerializeField] private UnityEvent onInteract; // The event, executing each registered action from the list above
        [SerializeField] private UnityEvent afterInteract; // Event, executing after OnInteract
        [SerializeField] private UnityEvent againInteractable; // Event getting called, once its able to fire events again

        private void Awake() {
            if (Button != null) Button.onClick.AddListener(Interact);
        }

        /// <summary>
        ///     Gets called upon timer reset...
        ///     Re-Enables the interaction
        /// </summary>
        public void Reset() {
            Interactable = true;
            AgainInteractable.Invoke();
        }


        ////////////////////////
        /// Main
        ////////////////////////


        /// <summary>
        ///     Gets called once a click happens at that UI-Element.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) { Interact(); }

        ////////////////////////
        /// Properties
        ////////////////////////


        /// <summary>
        ///     The button we listen to, if not set... we listen for real interaction with the whole object instead.
        /// </summary>
        public Button Button {
            get => button;
            set => button = value;
        }

        /// <summary>
        ///     Returns true if this is interactable
        /// </summary>
        public bool Interactable {
            get => interactable;
            set => interactable = value;
        }

        /// <summary>
        ///     The cooldown for each click in seconds
        /// </summary>
        public float CoolDownS {
            get => coolDownS;
            set => coolDownS = value;
        }

        /// <summary>
        ///     Event getting fired, before <see cref="OnInteract" />
        ///     Often used for hooked classes to determine or modify <see cref="OnInteract" />
        /// </summary>
        public UnityEvent BeforeInteract {
            get => beforeInteract;
            set => beforeInteract = value;
        }

        /// <summary>
        ///     The callback we use on interaction
        /// </summary>
        public UnityEvent OnInteract {
            get => onInteract;
            set => onInteract = value;
        }

        /// <summary>
        ///     Event getting fired, after <see cref="OnInteract" />
        ///     Often used for hooked classes to determine or modify <see cref="OnInteract" />
        /// </summary>
        public UnityEvent AfterInteract {
            get => afterInteract;
            set => afterInteract = value;
        }

        /// <summary>
        ///     The callback which gets called, once this is interactable again.
        /// </summary>
        public UnityEvent AgainInteractable {
            get => againInteractable;
            set => againInteractable = value;
        }

        /// <summary>
        ///     Calls the events, gets called once we interact with the object
        /// </summary>
        protected void Interact() {
            if (Interactable) {
                if (BeforeInteract != null) BeforeInteract.Invoke();
                if (OnInteract != null) OnInteract.Invoke();
                if (AfterInteract != null) AfterInteract.Invoke();

                Interactable = false;
                Invoke("Reset", CoolDownS);
            }
        }
    }
}