using System;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Systems.Reactive;
using Unity.Entities;
using UnityEngine;

namespace Script.Client.Mono.MVVM.PopUp {
    
    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    /// The view model of the standard options UI.
    /// Controlls the logic and when and how to update.
    /// </summary>
    [RequireComponent(typeof(OptionView), typeof(EcsEntity))]
    public class OptionViewModel : MonoBehaviour{

        private void Awake() {
            
            Database = ServiceLocator.Get<InternalDatabase>();
            Localizations = Database.GetDatabase("localizations");

            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DirtyReactiveSystem = EntityManager.World.GetOrCreateSystem<DirtyReactiveSystem>();

            OptionView = GetComponent<OptionView>();
            OptionModel = GetComponent<EcsEntity>();

            if (OptionModel.EntityReference != Entity.Null) Initialize();
            else OptionModel.OnReference += entity => Initialize();

            DirtyReactiveSystem.OnAdded += Refresh;
        }
        
        /// <summary>
        /// Gets called at start, when the data model was assigned
        /// </summary>
        public void Initialize() {

            var entity = OptionModel.EntityReference;
            var dirty = new Dirty();
            Refresh(ref entity, ref dirty);
        }

        /// <summary>
        /// Refreshes the ui and reassign data to its fields
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dirty"></param>
        public void Refresh(ref Entity entity, ref Dirty dirty) {

            if (!EntityManager.HasComponent<Option>(entity)) return;
            if (entity != OptionModel.EntityReference) return;

            var localization = EntityManager.GetComponentData<Localizations>(entity);
            var locals = localization.LocalizationsMap;
            
            if(locals.Count() <= 0) return;
            
            var optionTitle = Localizations.GetFromContentStorage<LocalisationContent>(locals["title"]).DefaultLocalisation;
            OptionView.Title.text = $"{optionTitle}";
        }

        private void OnDestroy() {

            // Remove delegate, otherwhise it stays forever
            DirtyReactiveSystem.OnAdded -= Refresh;
        }

        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Localizations { get; set; }

        public EntityManager EntityManager { get; set; }
        public DirtyReactiveSystem DirtyReactiveSystem { get; set; }

        public OptionView OptionView { get; set; }
        public EcsEntity OptionModel { get; set; }
    }
}