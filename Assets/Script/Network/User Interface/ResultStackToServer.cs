using ParallelOrigin.Core.ECS.Components;
using Script.Client.Mono.Entity;
using Script.Client.Mono.User_Interface.Stacks.Results;
using Unity.Entities;
using UnityEngine;

namespace Script.Network.User_Interface {

    /// <summary>
    ///     This script is used to listen for inserted inputs into the <see cref="resultStack" /> in order to redirect those to the server.
    /// </summary>
    [RequireComponent(typeof(EcsEntity))]
    [RequireComponent(typeof(ResultStack))]
    public class ResultStackToServer : MonoBehaviour {

        [SerializeField] private EcsEntity ecsEntity;
        private EntityManager _manager;

        private void Awake() {

            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            ecsEntity = GetComponent<EcsEntity>();
            var resultStack = GetComponent<ResultStack>().EventDictionary;
            resultStack.OnAdded += ResultToServer; 
        }

        private void ResultToServer(string key, string value) {

            var identity = _manager.GetComponentData<Identity>(ecsEntity.EntityReference);

            /*
            var packet = new SFSObject();
            packet.PutUtfString("key", key);
            packet.PutUtfString("value", value);
            packet.PutLong("id", identity.id);

            var sender = new Sender();
            sender.SendExtensionRequest("result", packet);*/
        }
    }
}