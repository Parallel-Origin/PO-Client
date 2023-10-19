using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.Entity;
using Script.Client.Mono.User_Interface.Components;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Script.Network.User_Interface {

    /// <summary>
    ///     Checks if an UI Element was clicked, then sends it to the Server
    /// </summary>
    [RequireComponent(typeof(UIClickListener))]
    [RequireComponent(typeof(EcsEntity))]
    public class UIClickToServer : MonoBehaviour {

        [SerializeField] private UIClickListener clickListener;
        [SerializeField] private EcsEntity ecsEntity;
        [SerializeField] private EntityManager _entityManager;

        private ClientNetwork _network;

        private void Awake() {
            clickListener = GetComponent<UIClickListener>();
            ecsEntity = GetComponent<EcsEntity>();
            _network = ServiceLocator.Get<ClientNetwork>();
        }

        private void Start() {

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            clickListener.OnInteract.AddListener(() => {

                // Find local player
                var localPlayer = _entityManager.FindLocalPlayer();
                var localIdentity = _entityManager.GetComponentData<Identity>(localPlayer);
                
                // The clicked ui element
                var entity = ecsEntity.EntityReference;
                var identity = _entityManager.GetComponentData<Identity>(entity);

                // Construct click command and send it to the server. 
                var command = new ClickCommand { Clicked = new EntityLink(identity.ID), Clicker = new EntityLink(localIdentity.ID) };
                _network.Send(ref command);
            });
        }
    }
}