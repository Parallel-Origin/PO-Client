using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Transform;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace Script.Client.Prototypers {
    
    /// <summary>
    /// A <see cref="Prototyper{I,T}"/> that clones <see cref="Entity"/>'s that represent mobs ingame.
    /// </summary>
    public class MobPrototyper : Prototyper<Entity> {

        private EntityManager _entityManager;
        private World _world;
        
        public override void Initialize() {
            
            ServiceLocator.Register(this);

            // ECS
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            var required = new List<ComponentType> {
                typeof(Identity), 
                typeof(Mesh),
                typeof(NetworkTransform),
                typeof(LocalTransform),
                typeof(NetworkRotation),
                typeof(Rotation),
                typeof(Movement),
                typeof(Animation),
                typeof(Mob)
            };
            var defaultMob = _entityManager.CreateArchetype(required.ToArray());

            // A wolve mob
            Register(1, () => _entityManager.CreateEntity(defaultMob), (ref Entity entity) => {
                
                var anim = new Animation {
                    ControllerID = 1,
                    OverridenAnimationClips = new UnsafeParallelHashMap<FixedString32Bytes, byte>(2, Allocator.Persistent),
                    BoolParams = new UnsafeParallelHashMap<FixedString32Bytes, bool>(4, Allocator.Persistent){},
                    Triggers = new UnsafeList<FixedString32Bytes>(4, Allocator.Persistent)
                };
                _entityManager.SetComponentData(entity, anim);
                _entityManager.SetComponentData(entity, new Movement{Speed = 0.004f, Target = Vector2d.Zero});
            });
        }

        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}