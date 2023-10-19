using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM {
    
    /// <summary>
    /// A view which stores references to all important items on the mainscreen
    /// </summary>
    public class MainScreenView : MonoBehaviour {

        [SerializeField] private Button quitButton;
        
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button messagesButton;
        [SerializeField] private Button createButton;
        
        [SerializeField] private Button skillsButton;
        [SerializeField] private Button characterButton;
        [SerializeField] private Button socialButton;
        
        [SerializeField] private Button travelButton;
        [SerializeField] private Button chatsButton;
        [SerializeField] private Button supportButton;

        public Button QuitButton {
            get => quitButton;
            set => quitButton = value;
        }

        public Button InventoryButton {
            get => inventoryButton;
            set => inventoryButton = value;
        }

        public Button MessagesButton {
            get => messagesButton;
            set => messagesButton = value;
        }

        public Button CreateButton {
            get => createButton;
            set => createButton = value;
        }

        public Button SkillsButton {
            get => skillsButton;
            set => skillsButton = value;
        }

        public Button CharacterButton {
            get => characterButton;
            set => characterButton = value;
        }

        public Button SocialButton {
            get => socialButton;
            set => socialButton = value;
        }

        public Button TravelButton {
            get => travelButton;
            set => travelButton = value;
        }

        public Button ChatsButton {
            get => chatsButton;
            set => chatsButton = value;
        }

        public Button SupportButton {
            get => supportButton;
            set => supportButton = value;
        }
    }
}