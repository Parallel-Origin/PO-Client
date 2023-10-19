using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Script.Client.Prototypers {
    /// <summary>
    ///     A <see cref="Prototyper{I,T}" /> for cloning and instantiating <see cref="Entity" /> which represents a Option for a Popup <see cref="PopupPrototyper" />
    /// </summary>
    public class OptionPrototyper : Prototyper<Entity> {
        
        private EntityManager _entityManager;
        private World _world;

        public override void Initialize() {
            ServiceLocator.Register(this);

            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            var required = new List<ComponentType> {
                typeof(Identity),
                typeof(Option),
                typeof(Mesh),
                typeof(Localizations),
                typeof(Child)
            };
            var defaultOption = _entityManager.CreateArchetype(required.ToArray());

            // Default option
            Register(1, () => _entityManager.CreateEntity(defaultOption), (ref Entity instance) => {});

            // visit option
            Register(2, () => _entityManager.CreateEntity(defaultOption), (ref Entity instance) => {});
            
            // Attack option
            Register(3, () => _entityManager.CreateEntity(defaultOption), (ref Entity instance) => {});
            
            // Pickup all option
            Register(4, () => _entityManager.CreateEntity(defaultOption), (ref Entity instance) => {});
            
            // Pickup option
            Register(5, () => _entityManager.CreateEntity(defaultOption), (ref Entity instance) => {});
        }

        public override void AfterClone(short typeID, ref Entity clonedInstance) {
            base.AfterClone(typeID, ref clonedInstance);

            _entityManager.SetComponentData(clonedInstance, new Identity {ID = 0, Type = "ui_option:" + typeID});
            _entityManager.SetComponentData(clonedInstance, new Mesh {ID = 1, Instantiate = true});
            _entityManager.SetComponentData(clonedInstance, new Localizations {LocalizationsMap = new UnsafeParallelHashMap<FixedString32Bytes, short>(4, Allocator.Persistent)});
            _entityManager.SetComponentData(clonedInstance, new Child());
        }
        
        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}