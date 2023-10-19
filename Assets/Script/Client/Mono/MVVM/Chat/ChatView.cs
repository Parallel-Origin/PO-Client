using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Chat {
    
    /// <summary>
    /// A view which stores references to the most important UI-Elements for the chat screens.
    /// </summary>
    public class ChatView : MonoBehaviour {

        [SerializeField] private RectTransform listTransform;
        [SerializeField] private TMP_InputField chatField;
        [SerializeField] private Button sendButton;

        /// <summary>
        /// The rect transform referencing the list where we place the chat messages 
        /// </summary>
        public RectTransform ListTransform {
            get => listTransform;
            set => listTransform = value;
        }

        /// <summary>
        /// The chat input field we write our message into
        /// </summary>
        public TMP_InputField ChatField {
            get => chatField;
            set => chatField = value;
        }

        /// <summary>
        /// The button we press to send our message 
        /// </summary>
        public Button SendButton {
            get => sendButton;
            set => sendButton = value;
        }
    }
}