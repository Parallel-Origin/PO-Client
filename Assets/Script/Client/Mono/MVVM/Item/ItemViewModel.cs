using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Mono.MVVM.Inventory;
using Script.Extensions;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Script.Client.Mono.MVVM.Item {
    
    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    /// A component that represents the UI-controller for <see cref="ItemView"/> and uses the data structure <see cref="EcsEntity"/> to fill the view with data.
    /// </summary>
    [RequireComponent(typeof(ItemView), typeof(EcsEntity))]
    public class ItemViewModel : MonoBehaviour {

        private void Awake() {

            Database = ServiceLocator.Get<InternalDatabase>();
            Icons = Database.GetDatabase("icons");
            
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            ItemView = GetComponent<ItemView>();
            ItemModel = GetComponent<EcsEntity>();

            if (ItemModel.EntityReference != Entity.Null) Initialize();
            else ItemModel.OnReference += entity => Initialize();
        }

        private void Initialize() {

            // Get icon component and find the fitting sprite
            var sprite = EntityManager.GetComponentData<ParallelOrigin.Core.ECS.Components.Sprite>(ItemModel.EntityReference);
            var spriteAsset = Icons.GetContentStorage(sprite.ID).Get<SpriteContent>().Sprite;
            
            spriteAsset.LoadAssetAsyncIfValid<Sprite>(handle => ItemView.SpriteRenderer.sprite = handle.Result);
        }
        
        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Icons { get; set; }
        public EntityManager EntityManager { get; set; }
        public ItemView ItemView { get; set; }
        public EcsEntity ItemModel { get; set; }
    }
}