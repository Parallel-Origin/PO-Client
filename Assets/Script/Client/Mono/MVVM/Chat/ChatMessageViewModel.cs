using ParallelOrigin.Core.Network;
using UnityEngine;

namespace Script.Client.Mono.MVVM.Chat {
    
    /// <summary>
    /// An simple view model for a chat message which controlls its logic and manages it. 
    /// </summary>
    [RequireComponent(typeof(ChatMessageView))]
    public class ChatMessageViewModel : MonoBehaviour{
        
        private void Awake() {
            ChatMessageView = GetComponent<ChatMessageView>();
        }

        public void Initialize() {

            ChatMessageView.UsernameTextField.text = ChatMessageCommand.SenderUsername;
            ChatMessageView.MessageTextField.text = ChatMessageCommand.Message;
        }

        public ChatMessageView ChatMessageView { get; set; }
        public ChatMessageCommand ChatMessageCommand { get; set; }
    }
}