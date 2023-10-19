using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.Activities.Interaction;
using Script.Client.Mono.Entity;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Script.Network.Activities.Interaction {

    /// <summary>
    ///  This class listens for a Touch event from <see cref="TouchListener" /> and sends the id and type to the server
    /// </summary>
    [RequireComponent(typeof(TouchListener))]
    public class ClickToServer : MonoBehaviour {

        private ClientNetwork _network;
        private EntityManager _manager;
        
        private TouchListener _touchListener;

        private void Start() {

            ServiceLocator.Wait<ClientNetwork>(o => _network = (ClientNetwork)o);
            
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _touchListener = GetComponent<TouchListener>();

            _touchListener.OnTaps += (size,hits) => {

                var hit = GetFirstEntity(size, hits);
                if (!hit.collider) return;
                
                // Find local player
                var gameObject = hit.collider.gameObject;
                var localPlayer = _manager.FindLocalPlayer();
                var localIdentity = _manager.GetComponentData<Identity>(localPlayer);
                
                // Find clicked entity
                var entity = gameObject.GetComponent<EcsEntity>().EntityReference;
                var identity = _manager.GetComponentData<Identity>(entity);

                // Possible errors which may fuck with the server
                if(!_manager.Exists(localPlayer))
                    Debug.Log("Clicked but local player couldnt be found, its entity is null");
                
                if(!_manager.Exists(entity))
                    Debug.Log("Clicked but entity couldnt be found, its entity is null");
                
                // Send click to server
                var clickCommand = new ClickCommand {
                    Clicker = new EntityLink(localIdentity.ID), 
                    Clicked = new EntityLink(identity.ID)
                };
                
                _network.Send(ref clickCommand);
            };
        }

        /// <summary>
        /// Searches the first hittable entity and returns it. 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public RaycastHit GetFirstEntity(int size, RaycastHit[] hits) {

            for (var index = 0; index < size; index++) {

                ref var hit = ref hits[index];
                if (hit.collider.GetComponent<EcsEntity>()) return hit;
            }

            return default;
        }
    }
}