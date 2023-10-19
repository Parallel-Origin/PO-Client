using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Script.Client.Prototypers {
    
    /// <summary>
    ///     A <see cref="Prototyper{I,T}" /> for cloning and instantiating <see cref="Entity" /> which represents a popup in the game
    /// </summary>
    public class PopupPrototyper : Prototyper<Entity> {
    
        private EntityManager _entityManager;
        private World _world;

        public override void Initialize() {
            ServiceLocator.Register(this);

            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            var required = new List<ComponentType> {
                typeof(Identity),
                typeof(Popup),
                typeof(Mesh),
                typeof(Localizations),
                typeof(Parent)
            };
            var defaultPopUp = _entityManager.CreateArchetype(required.ToArray());

            // Tree popup
            Register(1, () => _entityManager.CreateEntity(defaultPopUp), (ref Entity instance) => {});
            
            // Building popup
            Register(2, () => _entityManager.CreateEntity(defaultPopUp), (ref Entity instance) => {});
            
            // Wolfe popup
            Register(3, () => _entityManager.CreateEntity(defaultPopUp), (ref Entity instance) => {});
            
            // Gold item popup
            Register(4, () => _entityManager.CreateEntity(defaultPopUp), (ref Entity instance) => {});
            
            // Wood item popup
            Register(5, () => _entityManager.CreateEntity(defaultPopUp), (ref Entity instance) => {});
            
            // Pickup amount popup
            Register(6, () => _entityManager.CreateEntity(defaultPopUp), (ref Entity instance) => {});
        }

        public override void AfterClone(short typeID, ref Entity clonedInstance) {
            base.AfterClone(typeID, ref clonedInstance);

            _entityManager.SetComponentData(clonedInstance, new Identity {ID = 0, Type = "ui_popup:" + typeID});
            _entityManager.SetComponentData(clonedInstance, new Mesh {ID = 1, Instantiate = true});
            _entityManager.SetComponentData(clonedInstance, new Localizations {LocalizationsMap = new UnsafeParallelHashMap<FixedString32Bytes, short>(4, Allocator.Persistent)});
            _entityManager.SetComponentData(clonedInstance, new Parent{Children = new UnsafeList<EntityLink>(2, Allocator.Persistent)});
        }
        
        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}