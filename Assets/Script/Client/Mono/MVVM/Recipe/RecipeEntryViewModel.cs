using System;
using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Mono.User_Interface.Screens;
using Script.Extensions;
using Script.Network;
using Script.Server;
using Unity.Entities;
using UnityEngine;
using Mesh = ParallelOrigin.Core.ECS.Components.Mesh;
using Sprite = UnityEngine.Sprite;

namespace Script.Client.Mono.MVVM.Recipe {
    
    using SpriteComponent = ParallelOrigin.Core.ECS.Components.Sprite;
    using Recipe = ParallelOrigin.Core.ECS.Components.Interactions.Recipe;
    
    /// <summary>
    /// The view model of the standard recipe UI.
    /// Controlls the logic and when and how to update.
    /// </summary>
    [RequireComponent(typeof(RecipeEntryView), typeof(EcsEntity))]
    public class RecipeEntryViewModel : MonoBehaviour{

        private void Awake() {
            
            Database = ServiceLocator.Get<InternalDatabase>();
            Meshes = Database.GetDatabase("meshes");
            Localizations = Database.GetDatabase("localizations");
            Icons = Database.GetDatabase("icons");

            GameScreenManager = ServiceLocator.Get<GameScreenManager>();
            Network = ServiceLocator.Get<ClientNetwork>();
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
   
            RecipeEntryView = GetComponent<RecipeEntryView>();
            RecipeEntryModel = GetComponent<EcsEntity>();

            if (RecipeEntryModel.EntityReference != Unity.Entities.Entity.Null) Initialize();
            else RecipeEntryModel.OnReference += entity => Initialize();
            
            RecipeEntryView.CreateButton.onClick.AddListener(Craft);
        }
        
        /// <summary>
        /// Gets called at start to fill initial values
        /// </summary>
        public void Initialize() {
            var dirty = new Dirty();
            Refresh(RecipeEntryModel.EntityReference, ref dirty);    
        }

        /// <summary>
        /// Gets called everytime our datastructure <see cref="EcsEntity"/> refreshes ( marked with <see cref="Dirty"/> ).
        /// Updates our UI.
        /// </summary>
        /// <param name="entity">The entity that refreshed</param>
        /// <param name="dirty"></param>
        public void Refresh(Unity.Entities.Entity entity, ref Dirty dirty) {

            if (entity != RecipeEntryModel.EntityReference) return;
            
            // Acess components
            var recipe = EntityManager.GetComponentData<Recipe>(RecipeEntryModel.EntityReference);
            var sprite = EntityManager.GetComponentData<SpriteComponent>(RecipeEntryModel.EntityReference);
            var localisation = EntityManager.GetComponentData<Localizations>(RecipeEntryModel.EntityReference);

            // Acess database
            var buildingName = Localizations.GetContentStorage(localisation.LocalizationsMap["buildingName"]).Get<LocalisationContent>().DefaultLocalisation;
            var buildingDescribtion = Localizations.GetContentStorage(localisation.LocalizationsMap["buildingDescription"]).Get<LocalisationContent>().DefaultLocalisation;
            var buildingIcon = Icons.GetContentStorage(sprite.ID).Get<SpriteContent>().Sprite;
            
            // Spawn childs
            if (Ingredients == null || Ingredients.Count <= 0) {
                
                for (var index = 0; index < recipe.Ingredients.Length; index++) {

                    var ingredient = recipe.Ingredients[index];
                    var ingredientMesh = Meshes.GetContentStorage(12).Get<GameObjectContent>().Representation;
                    
                    ingredientMesh.InstantiateAsync(RecipeEntryView.IngredientsTransform).Completed += handle => {

                        var ingredientViewModel = handle.Result.GetComponent<RecipeIngredientViewModel>();
                        ingredientViewModel.RecipeIngredientModel = ingredient;
                        ingredientViewModel.Initialize();
                        Ingredients[entity] = handle.Result;
                    };
                }
            }

            // Fill data into the view
            RecipeEntryView.TitleTextField.text = $"{buildingName}";
            RecipeEntryView.DescribtionTextField.text = $"{buildingDescribtion}";
            buildingIcon.LoadAssetAsyncIfValid<Sprite>(handle => RecipeEntryView.Icon.sprite = handle.Result);
        }

        /// <summary>
        /// Returns true if all ingredients in this recipe are available in the players pocket. 
        /// </summary>
        /// <returns></returns>
        private bool Valid() {

            var valid = true;
            var childs = RecipeEntryView.IngredientsTransform.childCount;

            for (var index = 0; index < childs; index++) {
                
                var child = RecipeEntryView.IngredientsTransform.GetChild(index);
                var ingredientViewModel = child.GetComponent<RecipeIngredientViewModel>();

                if (ingredientViewModel.Valid) continue;
                valid = false;
                break;
            }

            return valid;
        }
        
        /// <summary>
        /// Crafts the recipe by informing the server about this. 
        /// </summary>
        private void Craft() {

            // Prevent from crafting if recipe cant be... crafted
            if (!Valid()) {
                Debug.Log("Popup ? :)");
                return;
            }

            var en = EntityManager;
            var localPlayer = en.FindLocalPlayer();
            
            // Get player and recipe identitys
            var localPlayerIdentity = en.GetComponentData<Identity>(localPlayer);
            var recipeIdentity = en.GetComponentData<Identity>(RecipeEntryModel.EntityReference);
            
            // Construct build command and send it to the server
            var buildCommand = new BuildCommand {
                Builder = new EntityLink(localPlayerIdentity.ID),
                Recipe = recipeIdentity.Type.ToStringCached()
            };
            Network.Send(ref buildCommand);
            
            // Close all gamscreens
            GameScreenManager.CloseAll();
        }

        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Meshes { get; set; }
        public IInternalDatabase Localizations { get; set; }
        public IInternalDatabase Icons { get; set; }
        
        
        public GameScreenManager GameScreenManager { get; set; }
        public ClientNetwork Network { get; set; }
        public EntityManager EntityManager { get; set; }
        
        public RecipeEntryView RecipeEntryView { get; set; }
        public EcsEntity RecipeEntryModel { get; set; }

        /// <summary>
        /// The visible recipe entities
        /// </summary>
        public IDictionary<Unity.Entities.Entity, GameObject> Ingredients { get; set; } = new Dictionary<Unity.Entities.Entity, GameObject>();
    }
}