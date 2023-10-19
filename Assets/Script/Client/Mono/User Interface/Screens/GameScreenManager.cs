using System.Collections.Generic;
using System.Linq;
using ParallelOrigin.Core.Base.Classes.Pattern.Observer;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.Base.Interfaces.Observer;
using Script.Client.Mono.User_Interface.Stacks;
using UnityEngine;
using Event = Script.Client.Mono.Event;

namespace Script.Client.Mono.User_Interface.Screens {
    /// <summary>
    ///  This class works hand in hand with <see cref="UIManager" /> for listeneing to new UI-Elements and categories
    ///  It manages all ingame screens using a <see cref="Stacks.UIStack" /> for the screen flow
    ///  Contains several methods for passing <see cref="Props" /> to certain gamescreens
    ///  Fires several global events...
    ///  openedScreen | name
    ///  closedScreen | name
    /// </summary>
    [RequireComponent(typeof(UIStack))]
    public class GameScreenManager : MonoBehaviour {
        
        [SerializeField] private GameScreen mainMenu;
        [SerializeField] private List<IGameScreen> _screens = new List<IGameScreen>();
        
        [SerializeField] private IEventHandler _eventHandler;
        [SerializeField] private UIManager uiManagement;
        [SerializeField] private UIStack uiStack;

        private void Awake() {
            ServiceLocator.Register(this);
            uiStack = GetComponent<UIStack>();
        }

        private void Start() {

            uiManagement = ServiceLocator.Get<UIManager>();
            _eventHandler = uiManagement.EventHandler;

            // Listen for new UI-Elements, if screen... get added to our tracking list
            uiManagement.OnNewElement += Add;

            // Add screen toogling, for invoking screen events
            uiStack.OnVisible += (key, gm) => {
                Toggle(key, gm);
                OpenEvent(key);
            };
            uiStack.OnInvisible += (key, gm) => {
                Toggle(key, gm);
                CloseEvent(key);
            };
        }
        
        /// <summary>
        ///     Adds a screen to the list and makes sure it lands on the stack, if it was previous opened
        /// </summary>
        /// <param name="element"></param>
        protected void Add(IUIElement element) {
            
            var parent = element.AsEntity.AsChild.Parent;
            if (parent == null || parent.GetComponent<IGameScreen>() == null) return;

            // Previous open screens, should stay open on game start ( Login )
            var screen = parent.GetComponent<IGameScreen>();
            if (screen.ScreenCanvas.enabled) UIStack.Show(screen.Name, screen.Screen.gameObject);

            _screens.Add(screen);
        }

        /// <summary>
        /// Searches an gamescreen by its name and returns it
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IGameScreen Get(string name) {
            
            var gameScreen = Screens.Find(screen => screen.Name.Equals(name));
            return gameScreen;
        }

        /// <summary>
        ///  Finds and opens a gamescreen by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        public void Open(string name) {
            
            // Open new screen
            if (!Screens.Any(screen => screen.Name.Equals(name))) return;

            var gameScreen = Get(name);
            UIStack.Show(name, gameScreen.Screen.gameObject);
        }

        /// <summary>
        ///  Opens a gamescreen by its name and passes a set of attributes along.
        ///  This is used for configurating a screen being opened ( title... describtion... etc. )
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        public void Open(string name, IParams param) {
            
            // Open new screen
            if (!Screens.Any(screen => screen.Name.Equals(name))) return;

            var gameScreen = Screens.Find(screen => screen.Name.Equals(name));
            gameScreen.OnPropsReceived.Invoke(param);
            UIStack.Show(name, gameScreen.Screen.gameObject);
        }

        /// <summary>
        /// Closes all screens
        /// </summary>
        public void CloseAll() {
            UIStack.HideAll();
        }

        /// <summary>
        ///     Toggles a screen from visible to invisible and the other way around.
        ///     Does not really close that screen, only makes it invisible.
        /// </summary>
        /// <param name="key">The screen key</param>
        /// <param name="gm"></param>
        public void Toggle(string key, GameObject gm) {
            
            var parent = gm.GetComponent<IUIElement>().AsEntity.AsChild.Parent;
            var gameScreen = parent.GetComponent<IGameScreen>();
            gameScreen.Toogle();
        }

        /// <summary>
        ///     Closes all active screens and the main menu screen.
        ///     If those are already closed, it will simply upen up the main menu screen.
        /// </summary>
        public void ToggleMainMenu() {
            
            var inStack = UIStack.EventStack.CurrentElement != null && UIStack.EventStack.CurrentElement.Equals(mainMenu.Screen.gameObject) || UIStack.EventStack.ElementStack.Contains(mainMenu.Screen.gameObject);

            if (inStack) CloseAll();
            else Open(MainMenu.Name);
        }

        /// <summary>
        ///     The main menu
        /// </summary>
        public GameScreen MainMenu {
            get => mainMenu;
            set => mainMenu = value;
        }

        /// <summary>
        ///  The UI-Stack we use to show, hide our screens in a typical app way
        /// </summary>
        public UIStack UIStack {
            get => uiStack;
            set => uiStack = value;
        }

        /// <summary>
        ///     All registered screens
        /// </summary>
        public List<IGameScreen> Screens {
            get => _screens;
            set => _screens = value;
        }
        

        /// <summary>
        ///     Calls a global Event that a certain screen was opened.
        ///     openedScreen | name
        /// </summary>
        /// <param name="name"></param>
        protected void OpenEvent(string name) { _eventHandler?.Post(gameObject, new Event("openedScreen", new Params(name))); }

        /// <summary>
        ///     Calls a global Event that a certain screen was closed
        ///     closedScreen | name
        /// </summary>
        /// <param name="name"></param>
        protected void CloseEvent(string name) { _eventHandler?.Post(gameObject, new Event("closedScreen", new Params(name))); }
    }
}