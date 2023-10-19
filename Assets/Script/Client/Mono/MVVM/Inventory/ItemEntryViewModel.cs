using System;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Items;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

using UnityEngine.AddressableAssets;
using Sprite = UnityEngine.Sprite;

namespace Script.Client.Mono.MVVM.Inventory {
    
    using Item = ParallelOrigin.Core.ECS.Components.Item;
    using SpriteComponent = ParallelOrigin.Core.ECS.Components.Sprite;
    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    /// A component that represents the UI-controller for <see cref="ItemEntryView"/> and uses the data structure <see cref="EcsEntity"/> to fill the view with
    /// </summary>
    [RequireComponent(typeof(ItemEntryView))]
    [RequireComponent(typeof(EcsEntity))]
    public class ItemEntryViewModel : MonoBehaviour {

        // The key for the database lookup for localisations 
        private static readonly int EquipedLocalization = 21;

        // The color for the equiped localisation
        [SerializeField] private Color equipedColor;
        
        private void Awake() {

            Database = ServiceLocator.GetBySubType<IRegisterableInternalDatabase>();
            Localizations = Database.GetDatabase("localizations");
            Icons = Database.GetDatabase("icons");
            
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DirtyReactiveSystem = EntityManager.World.GetOrCreateSystem<DirtyReactiveSystem>();
            
            ItemEntryView = GetComponent<ItemEntryView>();
            ItemEntryModel = GetComponent<EcsEntity>();

            if (ItemEntryModel.EntityReference != Entity.Null) Initialize();
            else ItemEntryModel.OnReference += entity => Initialize();

            DirtyReactiveSystem.OnAdded += Refresh;
        }

        /// <summary>
        /// Gets called at start to fill initial values
        /// </summary>
        public void Initialize() {

            var entity = ItemEntryModel.EntityReference;
            var dirty = new Dirty();
            
            Refresh(ref entity, ref dirty);    
        }

        /// <summary>
        /// Gets called everytime our datastructure <see cref="EcsEntity"/> refreshes ( marked with <see cref="Dirty"/> ).
        /// Updates our UI.
        /// </summary>
        /// <param name="entity">The entity that refreshed</param>
        /// <param name="dirty"></param>
        public void Refresh(ref Entity entity, ref Dirty dirty) {

            if (!EntityManager.HasComponent<Item>(entity)) return;
            if (entity != ItemEntryModel.EntityReference) return;

            // Acess components
            var item = EntityManager.GetComponentData<Item>(ItemEntryModel.EntityReference);
            var sprite = EntityManager.GetComponentData<SpriteComponent>(ItemEntryModel.EntityReference);
            var localisation = EntityManager.GetComponentData<Localizations>(ItemEntryModel.EntityReference);
            var equiped = EntityManager.HasComponent<Equiped>(ItemEntryModel.EntityReference);
            
            // Acess database
            var itemName = Localizations.GetFromContentStorage<LocalisationContent>(localisation.LocalizationsMap["name"]).DefaultLocalisation;
            var icon = Icons.GetFromContentStorage<SpriteContent>(sprite.ID).Sprite;
            var equipedLocalization = Localizations.GetFromContentStorage<LocalisationContent>(EquipedLocalization).DefaultLocalisation;
            
            // This string gets attached in case the item we handle is equiped 
            var equipedColorString = ColorUtility.ToHtmlStringRGB(equipedColor);
            var equipedString = $"<#{equipedColorString}>[{equipedLocalization}]</color>";
            
            // Fill data into the view
            ItemEntryView.NameField.text = $"{item.Amount} {itemName} {(equiped ? equipedString : "")}";
            icon.LoadAssetAsyncIfValid<Sprite>(handle => ItemEntryView.Icon.sprite = handle.Result);
        }

        private void OnDestroy() {

            // Remove delegate
            DirtyReactiveSystem.OnAdded -= Refresh;
            
            // Release sprite which was allocated via adressables 
            Addressables.Release(ItemEntryView.Icon.sprite);
        }

        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Localizations { get; set; }
        public IInternalDatabase Icons { get; set; }
        
        public EntityManager EntityManager { get; set; }
        public DirtyReactiveSystem DirtyReactiveSystem { get; set; }
        
        public ItemEntryView ItemEntryView { get; set; }
        public EcsEntity ItemEntryModel { get; set; }
    }
}