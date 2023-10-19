using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Combat;
using ParallelOrigin.Core.ECS.Components.Items;
using ParallelOrigin.Core.ECS.Components.Transform;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Script.Client.Prototypers {
    
    /// <summary>
    ///  A <see cref="Prototyper{I,T}" /> for cloning and instantiating <see cref="Entity" /> which represents a player in the game.
    /// </summary>
    public class CharacterPrototyper : Prototyper<Entity> {
        
        private EntityManager _entityManager;
        private World _world;

        public override void Initialize() {
            
            ServiceLocator.Register(this);

            // ECS
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            var required = new List<ComponentType> {
                typeof(Identity),
                typeof(Character),
                typeof(Mesh),
                typeof(NetworkTransform),
                typeof(LocalTransform),
                typeof(NetworkRotation),
                typeof(Rotation),
                typeof(Movement),
                typeof(Inventory),
                typeof(Health),
                typeof(Animation)
            };
            var defaultPlayer = _entityManager.CreateArchetype(required.ToArray());
            
            
            // The default player
            Register(1, () => _entityManager.CreateEntity(defaultPlayer), (ref Entity entity) => {
                
                // Setting up mesh
                _entityManager.SetComponentData(entity, new Mesh {ID = 6, Instantiate = true});
                _entityManager.SetComponentData(entity, new Animation {
                    ControllerID = 1,
                    OverridenAnimationClips = new UnsafeParallelHashMap<FixedString32Bytes, byte>(2, Allocator.Persistent),
                    BoolParams = new UnsafeParallelHashMap<FixedString32Bytes, bool>(4, Allocator.Persistent),
                    Triggers = new UnsafeList<FixedString32Bytes>(4, Allocator.Persistent)
                });
            });
        }

        public override void AfterClone(short typeID, ref Entity clonedInstance) {
            base.AfterClone(typeID, ref clonedInstance); 
            
            _entityManager.SetComponentData(clonedInstance, new NetworkRotation{Value = quaternion.identity});
            _entityManager.SetComponentData(clonedInstance, new Rotation{Value = quaternion.identity});
        }

        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.SetComponentData(clonedInstance, new Identity {ID = 0, Type = "1:" + typeID});
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}