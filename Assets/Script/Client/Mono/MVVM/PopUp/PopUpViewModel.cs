using System;
using System.Text.RegularExpressions;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Script.Client.Mono.MVVM.PopUp {
    
    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    /// The view model of the standard popups UI.
    /// Controlls the logic and when and how to update.
    /// TODO : Vllt wäre es besser wenn das PopUp seine Options visuell reinspawnt und das nicht bereits im Mesh als true gesetzt ist...
    /// </summary>
    [RequireComponent(typeof(PopUpView), typeof(EcsEntity))]
    public class PopUpViewModel : MonoBehaviour{

        private void Awake() {

            Database = ServiceLocator.Get<InternalDatabase>();
            Localizations = Database.GetDatabase("localizations");

            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DirtyReactiveSystem = EntityManager.World.GetOrCreateSystem<DirtyReactiveSystem>();
            GameObjectReactiveSystem = EntityManager.World.GetOrCreateSystem<GameObjectReactiveSystem>();
            
            PopUpView = GetComponent<PopUpView>();
            PopUpModel = GetComponent<EcsEntity>();

            if (PopUpModel.EntityReference != Entity.Null) Initialize();
            else PopUpModel.OnReference += entity => Initialize();

            DirtyReactiveSystem.OnAdded += Refresh;
            GameObjectReactiveSystem.OnAdded += InsertOption;
        }

        /// <summary>
        /// Gets called at start, when the data model was assigned
        /// </summary>
        public void Initialize() {

            var entity = PopUpModel.EntityReference;
            var dirty = new Dirty();
            Refresh(ref entity, ref dirty);
        }

        /// <summary>
        /// Refreshes the ui and reassign data to its fields
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dirty"></param>
        public void Refresh(ref Entity entity, ref Dirty dirty) {

            if (!EntityManager.HasComponent<Popup>(entity)) return;
            if (entity != PopUpModel.EntityReference) return;
            
            // Get pairs
            var localizations = EntityManager.GetComponentData<Localizations>(entity);
            var locals = localizations.LocalizationsMap;
            var uniqueLocals = localizations.UniqueLocalizations;
            
            // Receive localisation
            var pair = locals["title"];
            if(pair == 0) return;
            
            var content = Localizations.GetFromContentStorage<LocalisationContent>(pair);
            var title = content.DefaultLocalisation;

            // Receive tags from string and replace them with the certain localisation.
            var tags = Regex.Matches(title, "({.*?})");
            foreach (Match matched in tags) {

                var match = matched.Value;
                var tag = match.Substring(1, match.Length - 2);

                // Either chain another localisation or a unique one ( fixed string )
                if (locals.ContainsKey(tag)) {

                    var localisation = locals[tag];
                    var tagContent = Localizations.GetFromContentStorage<LocalisationContent>(localisation);
                    var tagLocalised = tagContent.DefaultLocalisation;

                    title = title.Replace(match, tagLocalised);
                }
                else if (uniqueLocals.ContainsKey(tag)) {

                    var localisation = uniqueLocals[tag];
                    title = title.Replace(match, localisation.ToString());
                }
            }

            PopUpView.Title.text = $"{title}";
        }

        /// <summary>
        /// Gets called once a new option appeared and if it is a child of our popup, we insert it into our view
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="option"></param>
        public void InsertOption(ref Entity entity, ref GameObject option) {

            // Ignore entities without option and child
            if(!EntityManager.HasComponent<Option>(entity)) return;
            if(!EntityManager.HasComponent<Child>(entity)) return;

            // Make sure that the option is part of the popups children
            var identity = EntityManager.GetComponentData<Identity>(entity);
            var children = EntityManager.GetComponentData<Parent>(PopUpModel.EntityReference).Children;
            if (!children.Contains(identity.ID)) return; 
            
            option.transform.SetParent(PopUpView.OptionTransform, false);
        }

        private void OnDestroy() {

            // Clean up the method references, otherwhise they still will be called... even if the go is destroyed
            DirtyReactiveSystem.OnAdded -= Refresh;
            GameObjectReactiveSystem.OnAdded -= InsertOption;
        }

        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Localizations { get; set; }

        public EntityManager EntityManager { get; set; }
        public DirtyReactiveSystem DirtyReactiveSystem { get; set; }
        public GameObjectReactiveSystem GameObjectReactiveSystem { get; set; }
        
        public PopUpView PopUpView { get; set; }
        public EcsEntity PopUpModel { get; set; }
    }
}