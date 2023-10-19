using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Chat {
    
    /// <summary>
    /// A view for the fixed top chat in the normal gamescreen.
    /// </summary>
    public class FixedChatView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI username;
        [SerializeField] private TextMeshProUGUI chatMessage;
        [SerializeField] private Button button;

        /// <summary>
        /// The username 
        /// </summary>
        public TextMeshProUGUI Username {
            get => username;
            set => username = value;
        }

        /// <summary>
        /// The chat message textfield
        /// </summary>
        public TextMeshProUGUI ChatMessage {
            get => chatMessage;
            set => chatMessage = value;
        }

        /// <summary>
        /// The button of that fixed top chat,
        /// </summary>
        public Button Button {
            get => button;
            set => button = value;
        }
    }
}