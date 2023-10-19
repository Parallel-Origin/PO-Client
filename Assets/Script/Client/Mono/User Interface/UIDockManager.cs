using ParallelOrigin.Core.ECS.Components.Combat;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.User_Interface {
    /// <summary>
    ///     Manages the UI of the always visible dock at the bottom of the screen
    /// </summary>
    public class UIDockManager : MonoBehaviour {
        
        [SerializeField] private Slider healthbar;

        private EntityManager _manager;
        private EntityQuery _playerQuery;

        private void Awake() {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //playerQuery = manager.CreateEntityQuery(typeof(LocalPlayer), typeof(Health));
        }

        private void Update() {

            return;
            
            // Get local player
            var players = _playerQuery.ToEntityArray(Allocator.TempJob);

            if (players.Length <= 0) {
                players.Dispose();
                return;
            }

            var player = players[0];
            players.Dispose();

            // Update Health in Dock
            var health = _manager.GetComponentData<Health>(player);
            healthbar.value = health.CurrentHealth;
            healthbar.maxValue = health.MaxHealth;
        }
    }
}