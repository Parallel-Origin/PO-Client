using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Mono.User_Interface.Screens;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.Client.Mono.MVVM {
    
    /// <summary>
    /// A view model for the main screen to determine its logic. 
    /// </summary>
    [RequireComponent(typeof(GameScreen))]
    [RequireComponent(typeof(MainScreenView))]
    public class MainScreenViewModel : MonoBehaviour {

        [SerializeField] private AssetReference quitPopUp;
        
        private void Start() {
            
            GameScreenManager = ServiceLocator.Get<GameScreenManager>(); 
            GameScreen = GetComponent<GameScreen>();
            View = GetComponent<MainScreenView>();
            
            View.QuitButton.onClick.AddListener(OnQuitButton);
            View.InventoryButton.onClick.AddListener(OnInventoryButton);
            View.ChatsButton.onClick.AddListener(OnChatButton);
        }

        /// <summary>
        /// On quit, spawn a quit popup... 
        /// </summary>
        private void OnQuitButton() {
            quitPopUp.InstantiateAsync(GameScreen.Screen, false);
        }

        /// <summary>
        /// On inventory button press
        /// </summary>
        private void OnInventoryButton() {
            GameScreenManager.Open("inventory");
        }

        /// <summary>
        /// Once the chat button was pressed
        /// </summary>
        private void OnChatButton() {
            GameScreenManager.Open("globalchat");
        }

        public GameScreenManager GameScreenManager { get; set; }
        public GameScreen GameScreen { get; set; }
        public MainScreenView View { get; set; }
    }
}