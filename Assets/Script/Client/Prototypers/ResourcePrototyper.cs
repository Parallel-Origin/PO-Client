using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Combat;
using ParallelOrigin.Core.ECS.Components.Transform;
using Unity.Entities;
using Unity.Transforms;

namespace Script.Client.Prototypers {
    
    /// <summary>
    ///     A <see cref="Prototyper{I,T}" /> for cloning and instantiating <see cref="Entity" /> which represents a resource in the game.
    /// </summary>
    public class ResourcePrototyper : Prototyper<Entity> {
        
        private EntityManager _entityManager;
        private World _world;

        public override void Initialize() {
            ServiceLocator.Register(this);

            // ECS
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            var required = new List<ComponentType> {
                typeof(Identity),
                typeof(Resource),
                typeof(Mesh),
                typeof(Health),
                typeof(NetworkTransform),
                typeof(LocalTransform),
                typeof(NetworkRotation),
                typeof(Rotation)
            };
            var defaultResource = _entityManager.CreateArchetype(required.ToArray());

            // Southern tree
            Register(1, () => _entityManager.CreateEntity(defaultResource), (ref Entity instance) => {});
            // Southern tree
            Register(2, () => _entityManager.CreateEntity(defaultResource), (ref Entity instance) => {});
            // Southern tree
            Register(3, () => _entityManager.CreateEntity(defaultResource), (ref Entity instance) => {});
            // Southern tree
            Register(4, () => _entityManager.CreateEntity(defaultResource), (ref Entity instance) => {});
        }

        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.SetComponentData(clonedInstance, new Identity {ID = 0, Type = "2:" + typeID});
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}