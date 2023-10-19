using System.Collections.Generic;
using System.Threading.Tasks;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components.Items;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Mono.MVVM.Item;
using Script.Client.Mono.User_Interface.Components;
using Script.Client.Mono.User_Interface.Screens;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Mesh = ParallelOrigin.Core.ECS.Components.Mesh;

namespace Script.Client.Mono.MVVM.Inventory {

    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    ///  A class managing and representating the "Inventory"-Screen of the game.
    /// </summary>
    public class InventoryViewModel : MonoBehaviour {

        [SerializeField] private GameScreen gameScreen;
        [SerializeField] private RectTransform list;

        private void Awake() {

            ServiceLocator.Register(this);
            ServiceLocator.Wait<IRegisterableInternalDatabase>(o => {
                InternalDatabase = (IInternalDatabase) o;
                MeshDatabase = InternalDatabase.GetDatabase("meshes");
            });

            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void Initialize() {

            // Get manager, inventory and items 
            var entityManager = EntityManager;
            var localPlayer = entityManager.FindLocalPlayer();
            var inventory = EntityManager.GetComponentData<ParallelOrigin.Core.ECS.Components.Items.Inventory>(localPlayer);
            var items = inventory.Items;

            // Insert all items
            for (var index = 0; index < items.Length; index++) {

                var itemRef = inventory.Items[index];
                var item = itemRef.Resolve(ref entityManager);
                
                if(item == Entity.Null) continue;
                if(Items.ContainsKey(item)) continue;
                
                CreateItemEntry(item);
            }
        }

        public void Added(ref EntityLink added) {
            
            if (!gameScreen.Initialized) return;
            
            // Get manager, inventory and items 
            var entityManager = EntityManager;

            // Insert all items
            var item = added.Resolve(ref entityManager);
            
            if(item == Entity.Null) return;
            if(Items.ContainsKey(item)) return;
            
            CreateItemEntry(item);
        }
        
        public void Updated(ref EntityLink updated) {

            if (!gameScreen.Initialized) return;
            
            // Get manager, inventory and items 
            var entityManager = EntityManager;
            
            var item = updated.Resolve(ref entityManager);
            
            if(item == Entity.Null) return;
            if(!Items.ContainsKey(item)) return;

            var go = Items[item];
            var itemViewModel = go.GetComponent<ItemEntryViewModel>();
            itemViewModel.Initialize();
        }
        
        public void Removed(ref EntityLink removed) {
            
            if (!gameScreen.Initialized) return;
            
            // Get manager, inventory and items 
            var entityManager = EntityManager;
            
            var item = removed.Resolve(ref entityManager);
            
            if(item == Entity.Null) return;
            if(!Items.ContainsKey(item)) return;

            var go = Items[item];
            
            Addressables.ReleaseInstance(go);
            Items.Remove(item);
        }

        public void CreateItemEntry(Entity entity) {
            
            var mesh = EntityManager.GetComponentData<Mesh>(entity);

            // Getting assets & filling spawned items with null to prevent multiple same items.
            var meshContentStorage = MeshDatabase.GetContentStorage(mesh.ID);
            var meshAsset = meshContentStorage.Get<GameObjectContent>().Representation;

            // Initializing assets and the item-entry itself
            meshAsset.InstantiateAsync(list).Completed += async handle => {
                
                // Insert attributes inside the gameobject
                var itemEntity = handle.Result.GetComponent<EcsEntity>();
                itemEntity.EntityReference = entity;

                Items[entity] = handle.Result;
            };
        }

        public EntityManager EntityManager { get; set; }
        public AddedToInventoryReactiveSystem AddedReactiveSystem { get; set; }
        public RemovedFromInventoryReactiveSystem RemovedReactiveSystem { get; set; }
        
        public IInternalDatabase InternalDatabase { get; set; }
        public IInternalDatabase MeshDatabase { get; set; }

        /// <summary>
        /// The visible inventory items 
        /// </summary>
        public IDictionary<Entity, GameObject> Items { get; set; } = new Dictionary<Entity, GameObject>();
    }
}