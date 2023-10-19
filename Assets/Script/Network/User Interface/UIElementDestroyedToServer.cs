using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.Entity;
using Script.Client.Mono.Entity.Components;
using Script.Client.Mono.User_Interface;
using Unity.Entities;
using UnityEngine;

namespace Script.Network.User_Interface {

    /// <summary>
    ///  This component is used to intercept destroy callbacks from the <see cref="Callbacks" /> in order to send them to the server where they get processed.
    /// </summary>
    [RequireComponent(typeof(Callbacks))]
    [RequireComponent(typeof(EcsEntity))]
    public class UIElementDestroyedToServer : MonoBehaviour {

        private EcsEntity _ecsEntity;
        private Callbacks _callbacks;

        private ClientNetwork _network;

        private void Awake() {
            _callbacks = GetComponent<Callbacks>();
            _ecsEntity = GetComponent<EcsEntity>();
            _network = ServiceLocator.Get<ClientNetwork>();
        }

        private void Start() { _callbacks.OnDestroy += c => SendToServer(); }

        /// <summary>
        ///  Notifies the server that the <see cref="UIElement" /> was destroyed.
        /// </summary>
        protected void SendToServer() {
            
            // Get destroyed entity
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = _ecsEntity.EntityReference;
            var identity = manager.GetComponentData<Identity>(entity);

            // Send destruction command to server
            var command = new EntityCommand { Id = identity.ID, Type = string.Empty, Opcode = 5 };
            _network.Send(ref command);
        }
    }
}