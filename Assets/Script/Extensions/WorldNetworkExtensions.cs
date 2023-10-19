using LiteNetLib;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components.Transform;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.Map;
using Script.Client.Mono.MVVM.Chat;
using Script.Client.Mono.User_Interface.Screens;
using Unity.Entities;
using UnityEngine;

namespace Script.Extensions {
    
    /// <summary>
    /// A class containing extensions for the world network communication
    /// </summary>
    public static class WorldNetworkExtensions {


        /// <summary>
        /// Centers the local map around a certain position provided by the <see cref="MapCommand"/> from the server. 
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="command"></param>
        public static void CenterMap(MapCommand command, NetPeer peer) {
            
            var movementArea = ServiceLocator.Get<MovementArea>();
            movementArea.CenterAt(command.Position);
                
            Debug.Log($"Movementarea centered around {command.Position}");
        }

        /// <summary>
        /// Processes a <see cref="TeleportationCommand"/> and teleports a certain entity locally to the desired position. 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="peer"></param>
        public static void Teleport(TeleportationCommand command, NetPeer peer) {

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = command.Entity.Resolve(entityManager);
            entityManager.SetComponentData(entity, new NetworkTransform{ Pos = command.Position });
            entityManager.SetComponentData(entity, new LocalTransform{ Pos = command.Position });
        }
        
        /// <summary>
        /// Shows a received <see cref="ChatMessageCommand"/> from the server in all the chatscreens on the client 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="peer"></param>
        public static void ShowMessage(ChatMessageCommand command, NetPeer peer) {
            
            // Get chat screen and insert message
            var gameScreenManager = ServiceLocator.Get<GameScreenManager>();
            var fixedTopChat = ServiceLocator.Get<FixedChatViewModel>();
            
            var globalChatScreen = (GameScreen)gameScreenManager.Get("globalchat");
            var globalChatView = globalChatScreen.GetComponent<ChatViewModel>();
            
            globalChatView.Add(command);
            fixedTopChat.Show(command.SenderUsername, command.Message);
                
            Debug.Log($"Chatmessage [{command.Channel}]/{command.SenderUsername}:{command.Message}");
        }

        /// <summary>
        /// Shows a received batch of <see cref="ChatMessageCommand"/> from the server in all the chatscreens on the client 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="peer"></param>
        public static void ShowMessages(BatchCommand<ChatMessageCommand> command, NetPeer peer) {
            
            // Get chat screen and insert message
            var gameScreenManager = ServiceLocator.Get<GameScreenManager>();
            var fixedTopChat = ServiceLocator.Get<FixedChatViewModel>();
            
            var globalChatScreen = (GameScreen)gameScreenManager.Get("globalchat");
            var globalChatView = globalChatScreen.GetComponent<ChatViewModel>();

            for (var index = 0; index < command.Data.Length; index++) {

                var chatCommand = command.Data[index];
                globalChatView.Add(chatCommand); 
                fixedTopChat.Show(chatCommand.SenderUsername, chatCommand.Message);
                
                Debug.Log($"Chatmessage [{chatCommand.Channel}]/{chatCommand.SenderUsername}:{chatCommand.Message}");
            }
        }
    }
}