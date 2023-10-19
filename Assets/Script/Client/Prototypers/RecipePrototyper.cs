using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Interactions;
using Unity.Entities;

namespace Script.Client.Prototypers {
    /// <summary>
    ///     A <see cref="Prototyper{I,T}" /> that is used to clone entities representing <see cref="Recipe" />'s
    /// </summary>
    public class RecipePrototyper : Prototyper<Entity> {
        
        private EntityManager _entityManager;
        private World _world;

        public override void Initialize() {
            ServiceLocator.Register(this);

            // ECS
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            var required = new List<ComponentType> {typeof(Identity), typeof(Mesh), typeof(Sprite), typeof(Recipe), typeof(BuildingRecipe), typeof(Localizations)};
            var defaultRecipe = _entityManager.CreateArchetype(required.ToArray());

            // The default structure
            Register(1, () => _entityManager.CreateEntity(defaultRecipe), (ref Entity entity) => {});
        }

        public override void AfterInstanced(short typeID,ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}