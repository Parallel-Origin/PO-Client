using System;
using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

using Mesh = ParallelOrigin.Core.ECS.Components.Mesh;

namespace Script.Client.Mono.MVVM.Recipe {

    using Recipe = ParallelOrigin.Core.ECS.Components.Interactions.Recipe;
    
    /// <summary>
    ///     A class managing and representating the "Build"-Screen of the game.
    /// </summary>
    public class BuildViewModel : MonoBehaviour {

        [SerializeField] private Transform contentArea;

        private EntityManager _entityManager;

        private IInternalDatabase _internalDatabase;
        private IInternalDatabase _meshDatabase;

        private void Awake() {

            ServiceLocator.Register(this);
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Start() {

            _internalDatabase = ServiceLocator.Get<InternalDatabase>();
            _meshDatabase = _internalDatabase.GetDatabase("meshes");
        }

        public void Initialize() {
            
            var recipes = _entityManager.Find(typeof(Recipe));
            foreach (var entity in recipes) {

                // Get components
                var mesh = _entityManager.GetComponentData<Mesh>(entity);

                // Get storages
                var meshAsset = _meshDatabase.GetContentStorage(mesh.ID);
                var meshGameObject = meshAsset.Get<GameObjectContent>().Representation;

                // Initialize async and set up the recipe
                meshGameObject.InstantiateAsync(contentArea).Completed += handle => {

                    // Insert attributes inside the gameobject
                    var recipeEntity = handle.Result.GetComponent<EcsEntity>();
                    recipeEntity.EntityReference = entity;

                    Recipes[entity] = handle.Result;
                };
            }
        }

        /// <summary>
        /// Can get called if a item in the players inventory was updated to update the building screen.
        /// </summary>
        /// <param name="reference"></param>
        public void Updated() {

            // Update all recipes
            foreach (var kvp in Recipes) {

                var recipeViewModel = kvp.Value.GetComponent<RecipeEntryViewModel>();
                var ingredients = recipeViewModel.Ingredients;

                // Refresn their ingredients 
                foreach (var ingredient in ingredients) {

                    var ingredientViewModel = ingredient.Value.GetComponent<RecipeIngredientViewModel>();
                    ingredientViewModel.Refresh();
                }
            }
        }
        
        /// <summary>
        /// The visible recipe entities
        /// </summary>
        public IDictionary<Unity.Entities.Entity, GameObject> Recipes { get; set; } = new Dictionary<Unity.Entities.Entity, GameObject>();
    }
}