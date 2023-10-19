using System;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using ParallelOrigin.Core.Base.Classes;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Transform;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Mono.User_Interface;
using Script.Client.Systems.Lifecycle;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Entity = Unity.Entities.Entity;
using Mesh = ParallelOrigin.Core.ECS.Components.Mesh;
using Object = UnityEngine.Object;

namespace Script.Client.Systems.Graphical {

    /// <summary>
    /// A pool for adressables basically. 
    /// </summary>
    public class AdressableAssetsPool : UnityEngine.Pool.ObjectPool<GameObject> {
        
        public AdressableAssetsPool(
            GameObject go,
            Action<GameObject> actionOnGet = null,
            Action<GameObject> actionOnRelease = null,
            bool collectionCheck = true,
            int defaultCapacity = 100,
            int maxSize = 250
        ) : base(() => Object.Instantiate(go), actionOnGet, actionOnRelease, Object.Destroy, collectionCheck, defaultCapacity, maxSize) { }
    }
    
    /// <summary>
    ///     A system iterating over <see cref="GameObject" /> which are marked for being <see cref="Destroy" /> at the end of the frame.
    ///     Its purpose is to release the gameobjects which removes them from the game instantly.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameObjectSystem : SystemBase {

        private UIManager _uiManager;

        public static Dictionary<AssetReference, AdressableAssetsPool> Pools = new ();
        private UnityEngine.Transform _fieldTransform;
        
        protected override void OnCreate() {
            base.OnCreate();
            
            ServiceLocator.Wait<IRegisterableInternalDatabase>(o => {
                Database = (IInternalDatabase) o;
                Database = Database.GetDatabase("meshes");
            });
            
            // Additional stuff
            ServiceLocator.Wait<UIManager>(o => {
                _uiManager = (UIManager) o;
                _fieldTransform = _uiManager.Spawns["field"].transform;
            });

            ServiceLocator.Wait<AbstractMap>(o => Map = (AbstractMap) o);
        }

        protected override void OnUpdate() {
            
            var mercator = (Vector2d)Map.CenterMercator;
            
            // Initialize and spawn popups
            Entities.ForEach((Entity entity, ref Mesh mesh) => {

                if (!mesh.Instantiate) return;

                var contentStorage = Database.GetContentStorage(mesh.ID);
                var goContent = contentStorage.Get<GameObjectContent>();
                var prefab = goContent.Representation;

                // Spawns gameobject asset and assigns it to the component
                prefab.InstantiateAsync(_fieldTransform, false).Completed += handle => {
                    
                    // Assign MonoEntity and the go itself to the entity :)
                    var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var go = handle.Result;
                   
                    var monoEntity = go.GetComponent<EcsEntity>() ?? go.AddComponent<EcsEntity>();
                    monoEntity.Index = entity.Index;
                    monoEntity.EntityReference = entity;
                    manager.AddComponentObject(entity, go);
                };
            }).WithAll<MeshAdded, Popup>().WithNone<NetworkTransform>().WithStructuralChanges().WithoutBurst().Run();
            
            // Initialize and spawn options atm
            Entities.ForEach((Entity entity, ref Mesh mesh) => {

                if (!mesh.Instantiate) return;

                var contentStorage = Database.GetContentStorage(mesh.ID);
                var goContent = contentStorage.Get<GameObjectContent>();
                var prefab = goContent.Representation;

                // Spawns gameobject asset and assigns it to the component
                prefab.InstantiateAsync().Completed += handle => {

                    // Assign MonoEntity and the go itself to the entity :)
                    var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var go = handle.Result;
                    
                    var monoEntity = go.GetComponent<EcsEntity>() ?? go.AddComponent<EcsEntity>();
                    monoEntity.Index = entity.Index;
                    monoEntity.EntityReference = entity;
                    manager.AddComponentObject(entity, go);
                };
            }).WithAll<MeshAdded>().WithNone<NetworkTransform, Popup>().WithStructuralChanges().WithoutBurst().Run();
            
            // Initialize and spawn a prefab/gameobject with only network transform and no rotation
            Entities.ForEach((Entity entity, ref Mesh mesh, ref NetworkTransform networkTransform) => {

                if (!mesh.Instantiate) return;
                
                var contentStorage = Database.GetContentStorage(mesh.ID);
                var goContent = contentStorage.Get<GameObjectContent>();
                var prefab = goContent.Representation;
                
                // Spawns gameobject asset and assigns it to the component
                var pos = networkTransform.Pos.AsUnityPosition(mercator, Map.WorldRelativeScale);
                if (!Pools.TryGetValue(prefab, out var pool)) {

                    var loadedGo = prefab.LoadAssetAsync<GameObject>().WaitForCompletion();
                    pool = new (loadedGo, OnGet, OnRelease, false);
                    Pools[prefab] = pool;
                }
                
                var go = pool.Get();
                go.transform.position = pos;

                var monoEntity = go.GetComponent<EcsEntity>() ?? go.AddComponent<EcsEntity>();
                monoEntity.Index = entity.Index;
                monoEntity.EntityReference = entity;

                var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                manager.AddComponentObject(entity, go);
            }).WithAll<MeshAdded>().WithNone<NetworkRotation>().WithoutBurst().WithStructuralChanges().Run();
            
            // Initialize and spawn a prefab/gameobject with network transform and rotation
            Entities.ForEach((Entity entity, ref Mesh mesh, ref NetworkTransform networkTransform, ref NetworkRotation rotation) => {

                if (!mesh.Instantiate) return;
                
                var contentStorage = Database.GetContentStorage(mesh.ID);
                var goContent = contentStorage.Get<GameObjectContent>();
                var prefab = goContent.Representation;
                
                // Spawns gameobject asset and assigns it to the component
                var pos = networkTransform.Pos.AsUnityPosition(mercator, Map.WorldRelativeScale);
                if (!Pools.TryGetValue(prefab, out var pool)) {

                    var loadedGo = prefab.LoadAssetAsync<GameObject>().WaitForCompletion();
                    pool = new (loadedGo, OnGet, OnRelease, false);
                    Pools[prefab] = pool;
                }
                
                var go = pool.Get();
                go.transform.position = pos;
                go.transform.rotation = rotation.Value;
                
                var monoEntity = go.GetComponent<EcsEntity>() ?? go.AddComponent<EcsEntity>();
                monoEntity.Index = entity.Index;
                monoEntity.EntityReference = entity;

                var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                manager.AddComponentObject(entity, go);
            }).WithAll<MeshAdded>().WithoutBurst().WithStructuralChanges().Run();
        }

        private static void OnGet(GameObject go) {
            
            go.GetComponent<Renderer>().enabled = true;
            
            var collider = go.GetComponent<Collider>();
            var canvas = go.GetComponentInChildren<Canvas>();
            
            if(collider) collider.enabled = true;
            if (canvas) canvas.enabled = true;
        }

        private static void OnRelease(GameObject go) {
            
            go.GetComponent<Renderer>().enabled = false;
            
            var collider = go.GetComponent<Collider>();
            var canvas = go.GetComponentInChildren<Canvas>();
            
            if (collider) collider.enabled = false;
            if (canvas) canvas.enabled = false;
        }
        
        /// <summary>
        /// The internal database being used
        /// </summary>
        public IInternalDatabase Database { get; set; }
        
        /// <summary>
        /// The game map
        /// </summary>
        public AbstractMap Map { get; set; }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(DestroySystem))]
    public partial class GameObjectDestroySystem : SystemBase {

        private EndSimulationEntityCommandBufferSystem _endFrameCommandBufferSystem;

        protected override void OnCreate() {
            base.OnCreate();

            ServiceLocator.Wait<IRegisterableInternalDatabase>(o => {
                Database = (IInternalDatabase) o;
                Database = Database.GetDatabase("meshes");
            });
            
            _endFrameCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate() {

            var ecb = _endFrameCommandBufferSystem.CreateCommandBuffer();
            
            // Releases all destroys gameobjects and removes the gameobject 
            Entities.ForEach((Entity en, GameObject go, Mesh mesh) => {
                
                var contentStorage = Database.GetContentStorage(mesh.ID);
                var goContent = contentStorage.Get<GameObjectContent>();
                var prefab = goContent.Representation;

                // Either return to pool or release forever 
                if (GameObjectSystem.Pools.TryGetValue(prefab, out var pool)) pool.Release(go);
                else Addressables.ReleaseInstance(go);
                
                ecb.RemoveComponent<GameObject>(en);
            }).WithAll<Destroy>().WithoutBurst().Run();
        }
        
        /// <summary>
        /// The internal database being used
        /// </summary>
        public IInternalDatabase Database { get; set; }
    }
}