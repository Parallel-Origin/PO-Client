using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Observer;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using UnityEngine;

namespace Script.Client.Mono.User_Interface.Screens {
    /// <summary>
    ///  A component which can get attached to gameobjects to encapsulate <see cref="IGameScreenManager" /> flow methods
    ///   Allows controll of the screen flow, either moving towards another screen, or moving backwards.
    /// </summary>
    public class GameScreenTransistion : MonoBehaviour {
        
        [SerializeField] private List<Component> props;
        [SerializeField] private GameScreenManager gameScreenManager;

        private void Start() { gameScreenManager = ServiceLocator.Get<GameScreenManager>(); }

        /// <summary>
        ///  Opens the next screen
        ///  When props are used, it passes those props directly to the <see cref="GameScreen" />
        /// </summary>
        /// <param name="name"></param>
        public void Open(string name) {
            if (props.Count == 0) { gameScreenManager.Open(name); }
            else {
                var convertedProps = new IProps[props.Count];
                for (var index = 0; index < props.Count; index++)
                    convertedProps[index] = (IProps) props[index];

                var values = new object[convertedProps.Length];
                for (var index = 0; index < convertedProps.Length; index++)
                    values[index] = convertedProps[index].Value;

                var param = new Params(values);
                gameScreenManager.Open(name, param);
            }
        }

        /// <summary>
        ///     Toggles a screen from open to close... or from close to open
        /// </summary>
        /// <param name="name"></param>
        public void Toggle(string name) {
            var gameScreen = gameScreenManager.UIStack.EventStack.CurrentElement;
            if (gameScreen != null && gameScreen.GetComponent<IUIElement>().AsEntity.AsChild.Parent.GetComponent<IGameScreen>().Name.Equals(name)) Back();
            else Open(name);
        }

        /// <summary>
        ///     Closes all active screens and the main menu screen.
        ///     If those are already closed, it will simply upen up the main menu screen.
        /// </summary>
        public void ToggleMainMenu() { gameScreenManager.ToggleMainMenu(); }

        /// <summary>
        ///     Moves one screen back
        /// </summary>
        public void Back() { gameScreenManager.UIStack.Back(); }
    }
}