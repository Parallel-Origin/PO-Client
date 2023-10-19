using System;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.User_Interface.Screens;
using UnityEngine;

namespace Script.Client.Mono.MVVM.Chat {
    
    /// <summary>
    /// A view model for the fixed top chat. 
    /// </summary>
    [RequireComponent(typeof(FixedChatView))]
    public class FixedChatViewModel : MonoBehaviour{

        private void Awake() {
            
            ServiceLocator.Register(this);
            ChatView = GetComponent<FixedChatView>();
            ChatView.Button.onClick.AddListener(() => {

                var gameScreenManager = ServiceLocator.Get<GameScreenManager>();
                gameScreenManager.Open("globalchat");
            });
        }

        /// <summary>
        /// Shows or displays a certain message
        /// </summary>
        /// <param name="command"></param>
        public void Show(string user, string message) {
            ChatView.Username.text = user;
            ChatView.ChatMessage.text = message;
        }

        /// <summary>
        /// The chat view this view model makes use of to acess the ui elements
        /// </summary>
        public FixedChatView ChatView { get; set; }
    }
}