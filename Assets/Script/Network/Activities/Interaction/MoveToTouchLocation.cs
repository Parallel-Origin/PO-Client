using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.Activities.Interaction;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Script.Network.Activities.Interaction {

    /// <summary>
    ///     A component which hooks itself into <see cref="_touchListener" /> for moving the local player - ecs upon a double click
    /// </summary>
    [RequireComponent(typeof(TouchListener))]
    public class MoveToTouchLocation : MonoBehaviour {

        private ClientNetwork _network;
        private EntityManager _manager;
        
        private AbstractMap _map;
        private TouchListener _touchListener;

        private void Start() {

            ServiceLocator.Wait<ClientNetwork>(o => _network = (ClientNetwork)o);

            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _map = ServiceLocator.Get<AbstractMap>();
            _touchListener = GetComponent<TouchListener>();

            // Upon double touch on screen
            _touchListener.OnDoubleTap += hit => {

                // Find local player
                var localPlayer = _manager.FindLocalPlayer();
                var identity = _manager.GetComponentData<Identity>(localPlayer);
                var latlong = hit.point.GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);

                // Send double click command to server
                var command = new DoubleClickCommand {
                    Clicker = new EntityLink(identity.ID), 
                    Position = (ParallelOrigin.Core.Base.Classes.Vector2d)latlong
                };
                
                _network.Send(ref command);
            };
        }
    }
}