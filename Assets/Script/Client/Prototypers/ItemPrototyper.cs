using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Transform;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Script.Client.Prototypers {
    
    /// <summary>
    /// A <see cref="Prototyper{I,T}"/> for <see cref="Item"/> entities as inventory items.
    /// </summary>
    public class ItemPrototyper : Prototyper<Entity> {
        
        private EntityManager _entityManager;
        private World _world;

        // Localisations to share and save memory
        private static readonly Localizations GoldLocalisation = new Localizations {
            LocalizationsMap = new UnsafeParallelHashMap<FixedString32Bytes, short>(2, Allocator.Persistent){{"name", 2}}
        };       
        private static readonly Localizations WoodLocalisation = new Localizations {
            LocalizationsMap = new UnsafeParallelHashMap<FixedString32Bytes, short>(2, Allocator.Persistent){{"name", 23}}
        };
        
        public override void Initialize() {
            ServiceLocator.Register(this);

            // ECS
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            // Required components
            var required = new List<ComponentType> {
                typeof(Identity), 
                typeof(Item), 
                typeof(Mesh), 
                typeof(Sprite),
                typeof(Localizations)
            };
            var defaultItem = _entityManager.CreateArchetype(required.ToArray());

            // Gold item
            Register(1, () => _entityManager.CreateEntity(defaultItem), (ref Entity entity) => {
                
                _entityManager.SetComponentData(entity, GoldLocalisation);
            });
            
            // Wood item
            Register(2, () => _entityManager.CreateEntity(defaultItem), (ref Entity entity) => {
                
                _entityManager.SetComponentData(entity, WoodLocalisation);
            });
        }

        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.SetComponentData(clonedInstance, new Identity {ID = 0, Type = "3:" + typeID});
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
    
    /// <summary>
    /// A <see cref="Prototyper{I,T}"/> for <see cref="Item"/> entities as inventory items.
    /// </summary>
    public class ItemOnGroundPrototyper : Prototyper<Entity> {
        
        private EntityManager _entityManager;
        private World _world;

        // Localisations to share and save memory
        private static readonly Localizations GoldLocalisation = new Localizations {
            LocalizationsMap = new UnsafeParallelHashMap<FixedString32Bytes, short>(2, Allocator.Persistent){{"name", 2}}
        };       
        private static readonly Localizations WoodLocalisation = new Localizations {
            LocalizationsMap = new UnsafeParallelHashMap<FixedString32Bytes, short>(2, Allocator.Persistent){{"name", 23}}
        };
        
        public override void Initialize() {
            ServiceLocator.Register(this);

            // ECS
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            // Required components
            var required = new List<ComponentType> {
                typeof(Identity), 
                typeof(Item), 
                typeof(Mesh), 
                typeof(Sprite),
                typeof(Localizations),
                typeof(NetworkTransform)
            };
            var defaultItem = _entityManager.CreateArchetype(required.ToArray());

            // Gold item
            Register(1, () => _entityManager.CreateEntity(defaultItem), (ref Entity entity) => {
                
                _entityManager.SetComponentData(entity, GoldLocalisation);
            });
            
            // Wood item
            Register(2, () => _entityManager.CreateEntity(defaultItem), (ref Entity entity) => {
                
                _entityManager.SetComponentData(entity, WoodLocalisation);
            });
        }

        public override void AfterInstanced(short typeID, ref Entity clonedInstance) {
            base.AfterInstanced(typeID, ref clonedInstance);
            
            _entityManager.SetComponentData(clonedInstance, new Identity {ID = 0, Type = "6:" + typeID});
            _entityManager.AddComponentData(clonedInstance, new Prefab());   
        }
    }
}