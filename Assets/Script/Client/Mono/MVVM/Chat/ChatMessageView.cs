using TMPro;
using UnityEngine;

namespace Script.Client.Mono.MVVM.Chat {
    
    /// <summary>
    /// A View which stores references to all importan chat message elements 
    /// </summary>
    public class ChatMessageView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI usernameTextField;
        [SerializeField] private TextMeshProUGUI messageTextField;

        /// <summary>
        /// The username textfield
        /// </summary>
        public TextMeshProUGUI UsernameTextField {
            get => usernameTextField;
            set => usernameTextField = value;
        }

        /// <summary>
        /// The message textfield
        /// </summary>
        public TextMeshProUGUI MessageTextField {
            get => messageTextField;
            set => messageTextField = value;
        }
    }
}