using System;
using ParallelOrigin.Core.Base.Interfaces.Observer;
using Script.Client.Mono.Entity.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Script.Client.Mono.User_Interface.Screens {
    
    /// <summary>
    ///     A interface, defining a gameobject, which acts as a gamescreen for representing/managing one UI-Screen ingame.
    /// </summary>
    public interface IGameScreen {

        /// <summary>
        ///     Toogles the visibility of the GameScreen.
        /// </summary>
        void Toogle();

        /// <summary>
        ///     Opens the game-screens and makes it visible.
        /// </summary>
        void OpenScreen();

        /// <summary>
        ///     Closes the gamescreen and makes it invisible.
        /// </summary>
        void CloseScreen();

        /// <summary>
        ///     The screen name, used for navigation
        ///     Should be unique
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The Screen-Transform of the UI, not this Component !
        /// </summary>
        RectTransform Screen { get; set; }


        /// <summary>
        ///     If our screen is open
        /// </summary>
        bool Open { get; set; }

        /// <summary>
        ///     If our screen is able to get toogled
        /// </summary>
        bool Toogable { get; set; }

        /// <summary>
        ///     A callback getting fired, once this screen opens
        /// </summary>
        UnityEvent OnOpen { get; set; }

        /// <summary>
        ///     A callback getting fired once this screen closes
        /// </summary>
        UnityEvent OnBack { get; set; }

        /// <summary>
        ///     Gets called, when we receive props for being displayed.
        ///     Props are params, passed trough the <see cref="GameScreenManager" />
        /// </summary>
        Action<IParams> OnPropsReceived { get; set; }

        /// <summary>
        ///     The canvas attached to the UI-Screen
        ///     Used for hiding and showing up the screen
        /// </summary>
        Canvas ScreenCanvas { get; set; }

        /// <summary>
        ///     The screen raycaster, for screen interaction
        /// </summary>
        GraphicRaycaster ScreenRaycaster { get; set; }

        /// <summary>
        ///     A event, getting fired, when the screen gets opened for the first time
        /// </summary>
        UnityEvent OnFirstOpen { get; set; }
    }
    
    /// <summary>
    ///     This class is used to manage the different screens effectivly.
    ///     It requires atleast a go back button to work properly.
    ///     Gets registered to the gamescreen managers.
    /// </summary>
    [RequireComponent(typeof(Callbacks))]
    public class GameScreen : MonoBehaviour, IGameScreen {
        
        [SerializeField] private string name;               // The screens name for opening and closing it via the manager 
        
        [SerializeField] private RectTransform screen;      // The screen transform
        [SerializeField] private Canvas screenCanvas;       // The screen canvas for toogling the screen visibility
        private GraphicRaycaster _screenRaycaster;           // The screen raycaster for screen interaction
        private Callbacks _callbacks;                        // The callbacks
        
        private bool _initialized;
        private bool _open;                                  // For tracking internal if the screen is open
        [SerializeField] private bool toogable = true;      // If true, the screen can get toogled.

        [SerializeField] private UnityEvent onFirstOpen;
        [SerializeField] private UnityEvent onOpen;
        [SerializeField] private UnityEvent onBack;
        
        [SerializeField] private Action<IParams> _onPropsReceived;
        
        ///////////////////////////
        /// Construcors
        ///////////////////////////
        /// 
        private void Awake() { _callbacks = GetComponent<Callbacks>(); }

        /// <summary>
        ///     Sets up the gamescreen, assign back button and registers the gamescreen.
        /// </summary>
        protected virtual void Start() {
            
            // Changing element category
            var screenUI = Screen.gameObject;
            var uiElement = screenUI.AddComponent<UIElement>();
            uiElement.AsEntity.ElementCategory = "screen";
            uiElement.AsEntity.AsChild.Parent = gameObject;

            // Adding canvas for hiding/showing the screen without disable it
            if (!screenUI.GetComponent<Canvas>()) {
                screenCanvas = screenUI.AddComponent<Canvas>();
                screenCanvas.enabled = false;
            }
            else screenCanvas = screenUI.GetComponent<Canvas>(); 

            // Adding a raycaster to enable UI-Interaction
            _screenRaycaster = !screenUI.GetComponent<GraphicRaycaster>() ? screenUI.AddComponent<GraphicRaycaster>() : screenUI.GetComponent<GraphicRaycaster>();

            // Invoking callback for possible listeners
            _callbacks.OnStart(this);
        }


        /////////////////////////////////
        /// Main methods
        /////////////////////////////////


        /// <summary>
        ///     Toogles the visibility of the GameScreen.
        /// </summary>
        public void Toogle() {
            
            if (Open) CloseScreen();
            else OpenScreen();
        }


        /// <summary>
        ///     Opens the game-screens and makes it visible.
        ///     If the screen was never opened before, the "onFirstOpen" Event gets called.
        /// </summary>
        public void OpenScreen() {
            
            Open = true;
            ScreenCanvas.enabled = true;
            ScreenRaycaster.enabled = true;

            if (!_initialized) {
                onFirstOpen.Invoke();
                _initialized = true;
            }

            OnOpen?.Invoke();
        }

        /// <summary>
        ///     Closes the gamescreen and makes it invisible.
        /// </summary>
        public void CloseScreen() {
            
            Open = false;
            ScreenCanvas.enabled = false;
            ScreenRaycaster.enabled = false;

            OnBack?.Invoke();
        }


        /////////////////////////////////
        /// Properties
        /////////////////////////////////

        /// <summary>
        ///     The name of this screen, used for navigation between screens...
        ///     Needs to be unique
        /// </summary>
        public string Name {
            get => name;
            set => name = value;
        }

        /// <summary>
        ///     The Screen-Transform of the UI, not this Component !
        /// </summary>
        public RectTransform Screen {
            get => screen;
            set => screen = value;
        }

        /// <summary>
        ///     The canvas attached to the UI-Screen
        ///     Used for hiding and showing up the screen
        /// </summary>
        public Canvas ScreenCanvas {
            get => screenCanvas;
            set => screenCanvas = value;
        }

        /// <summary>
        ///     The screen raycaster, for screen interaction
        /// </summary>
        public GraphicRaycaster ScreenRaycaster {
            get => _screenRaycaster;
            set => _screenRaycaster = value;
        }

        /// <summary>
        /// If the screen was already opened once and was initialized
        /// </summary>
        public bool Initialized => _initialized;

        /// <summary>
        ///     If our screen is open
        /// </summary>
        public bool Open {
            get => _open;
            set => _open = value;
        }

        /// <summary>
        ///     If our screen is able to get toogled
        /// </summary>
        public bool Toogable {
            get => toogable;
            set => toogable = value;
        }

        /// <summary>
        ///     A event, getting fired, when the screen gets opened for the first time
        /// </summary>
        public UnityEvent OnFirstOpen {
            get => onFirstOpen;
            set => onFirstOpen = value;
        }

        /// <summary>
        ///     A callback getting fired, once this screen opens
        /// </summary>
        public UnityEvent OnOpen {
            get => onOpen;
            set => onOpen = value;
        }

        /// <summary>
        ///     Gets called, when we receive props for being displayed.
        ///     Props are params, passed trough the <see cref="GameScreenManager" />
        /// </summary>
        public Action<IParams> OnPropsReceived {
            get => _onPropsReceived;
            set => _onPropsReceived = value;
        }

        /// <summary>
        ///     A callback getting fired once this screen closes
        /// </summary>
        public UnityEvent OnBack {
            get => onBack;
            set => onBack = value;
        }
    }
}