using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Interactions;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;
using Sprite = UnityEngine.Sprite;
using Entity = Unity.Entities.Entity;

namespace Script.Client.Mono.MVVM.Recipe {
    
    using Inventory = ParallelOrigin.Core.ECS.Components.Items.Inventory;
    using Item = ParallelOrigin.Core.ECS.Components.Item;
    
    /// <summary>
    /// The view model of the standard recipe ingredients UI.
    /// Controlls the logic and when and how to update.
    /// Dont uses a ECS entity as the data model, instead a struct called <see cref="RecipeIngredientModel"/>
    /// </summary>
    [RequireComponent(typeof(RecipeIngredientView))]
    
    public class RecipeIngredientViewModel : MonoBehaviour {

        [ColorUsage(true)] 
        [SerializeField] private Color invalidColor;

        private void Awake() {
            
            Database = ServiceLocator.Get<InternalDatabase>();
            Localizations = Database.GetDatabase("localizations");
            Icons = Database.GetDatabase("icons");
            
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            RecipeIngredientView = GetComponent<RecipeIngredientView>();
        }
        
        /// <summary>
        /// Gets called at start to fill initial values
        /// </summary>
        public void Initialize() { Refresh(); }

        /// <summary>
        /// Gets called everytime our datastructure <see cref="EcsEntity"/> refreshes ( marked with <see cref="Dirty"/> ).
        /// Updates our UI.
        /// </summary>
        /// <param name="entity">The entity that refreshed</param>
        /// <param name="dirty"></param>
        public void Refresh() {
            
            // Check if dirty entity is item
            var manager = EntityManager;
            var player = manager.FindLocalPlayer();
            var inventory = EntityManager.GetComponentData<Inventory>(player);

            var itemEntity = inventory.GetItemByType(RecipeIngredientModel.Type);
            var item = itemEntity != Unity.Entities.Entity.Null ? EntityManager.GetComponentData<Item>(itemEntity) : new Item{Amount = 0};

            // Acess database
            var ingredientIcon = Icons.GetContentStorage(RecipeIngredientModel.Icon).Get<SpriteContent>().Sprite;
            var ingredientName = Localizations.GetContentStorage(RecipeIngredientModel.Localisation).Get<LocalisationContent>().DefaultLocalisation;

            // Validate ingredients and choose color
            Valid = item.Amount >= RecipeIngredientModel.Amount;
            var invalidColor = ColorUtility.ToHtmlStringRGB(this.invalidColor);

            // Set recipe fields
            RecipeIngredientView.NameTextField.text = $"{(Valid ? "" : $"<color=#{invalidColor}>")}{item.Amount}{(Valid ? "" : "</color>")} / {RecipeIngredientModel.Amount} {ingredientName}";
            ingredientIcon.LoadAssetAsyncIfValid<Sprite>( handle => RecipeIngredientView.Icon.sprite = handle.Result);
        }
        
        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Localizations { get; set; }
        public IInternalDatabase Icons { get; set; }
        
        public EntityManager EntityManager { get; set; }

        public RecipeIngredientView RecipeIngredientView { get; set; }
        public Ingredient RecipeIngredientModel { get; set; }
        
        /// <summary>
        /// Returns true if the local player has enough ingredients in his inventory. 
        /// </summary>
        public bool Valid { get; set; }
    }
}