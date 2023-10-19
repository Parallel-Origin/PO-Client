using System;
using System.Collections;
using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Extensions;
using Script.Network;
using Script.Server;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SocialPlatforms;
using Network = ParallelOrigin.Core.Network.Network;

namespace Script.Client.Mono.MVVM.Chat {
    
    /// <summary>
    /// A view model for a chat screen which makes use of the <see cref="ChatView"/> and controlls the logic of the screen. 
    /// </summary>
    [RequireComponent(typeof(ChatView))]
    public class ChatViewModel : MonoBehaviour {

        [SerializeField] private AssetReference chatMessage;

        public byte channel = 0;
        public int maxAmount = 10;
        
        private void Awake() {
            ChatView = GetComponent<ChatView>();
        }

        public void Initialize() {

            EntityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
            ClientNetwork = ServiceLocator.Get<ClientNetwork>();

            var en = EntityManager;
            LocalPlayer = en.FindLocalPlayer();
            ChatView.SendButton.onClick.AddListener(Send);
        }

        /// <summary>
        /// Sends a chat message to the server when the send button was clicked
        /// </summary>
        public void Send() {

            // Prevent sending empty messages
            if (ChatView.ChatField.text.Length <= 0) return;
            
            // Send message
            var identity = EntityManager.GetComponentData<Identity>(LocalPlayer);
            var charac = EntityManager.GetComponentData<ParallelOrigin.Core.ECS.Components.Character>(LocalPlayer);
            
            var chatMessageCommand = new ChatMessageCommand {
                Sender = identity.ID,
                SenderUsername = charac.Name.ToStringCached(),
                Message = ChatView.ChatField.text,
                Channel = channel,
                Date = DateTime.UtcNow
            };
            
            ClientNetwork.Send(ref chatMessageCommand);

            // Clear
            ChatView.ChatField.text = string.Empty;
            ChatView.ChatField.DeactivateInputField(true);
        }
        
        /// <summary>
        /// Adds an chat message to this screen for being displayed
        /// </summary>
        /// <param name="command"></param>
        public void Add(ChatMessageCommand command) {

            // Remove old chat messages, kinda like a circular buffer
            if (ChatMessages.Count >= maxAmount) {

                var chatMessageGo = ChatMessages.Dequeue();
                Addressables.ReleaseInstance(chatMessageGo);
            }
            
            // Spawn in new chat message
            chatMessage.InstantiateAsync(ChatView.ListTransform).Completed += handle => {

                // Init
                var model = handle.Result.GetComponent<ChatMessageViewModel>();
                model.ChatMessageCommand = command;
                model.Initialize();
                
                // Store
                ChatMessages.Enqueue(handle.Result);
            };
        }

        public ClientNetwork ClientNetwork { get; set; }
        public EntityManager EntityManager { get; set; }
        public Unity.Entities.Entity LocalPlayer { get; set; }
        
        public ChatView ChatView { get; set; }
        public Queue<GameObject> ChatMessages { get; set; } = new Queue<GameObject>(32);
    }
}