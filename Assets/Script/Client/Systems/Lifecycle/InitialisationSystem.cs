using ParallelOrigin.Core.ECS.Components;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Entities;

namespace Script.Client.Systems.Lifecycle {
    
    /// <summary>
    /// A system that initializes newly created entities and makes sure to mark them correctly.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InitialisationSystem : SystemBase {

        private OneFrameSystem _oneFrameSystem;
        private CreatedReactiveSystem _createdReactiveSystem;
        
        private EndInitializationEntityCommandBufferSystem _startBufferSystem;
        
        protected override void OnCreate() {
            base.OnCreate();

            _oneFrameSystem = World.GetOrCreateSystem<OneFrameSystem>();
            _createdReactiveSystem = World.GetOrCreateSystem<CreatedReactiveSystem>();
            
            _startBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();

            // Let those components only exist one frame
            _oneFrameSystem.Register(typeof(Created));
            _oneFrameSystem.Register(typeof(Dirty));

            _createdReactiveSystem.OnAdded += OnCreated;
        }

        protected override void OnUpdate() {
            
            var startCommandBuffer = _startBufferSystem.CreateCommandBuffer().AsParallelWriter();

            // Mark new entities as created and dirty 
            Entities.ForEach((ref Entity en, ref int entityInQueryIndex) => {
                
                startCommandBuffer.AddComponent<Dirty>(entityInQueryIndex, en);
            }).WithAll<Identity, Created>().ScheduleParallel();
            
            _startBufferSystem.AddJobHandleForProducer(Dependency);
        }

        /// <summary>
        /// Gets invoked by the <see cref="_createdReactiveSystem"/> once an <see cref="Identity"/> was attached to an entity.
        /// Caches the <see cref="Identity.ID"/> for fast lookup and find operations. 
        /// </summary>
        /// <param name="en"></param>
        /// <param name="id"></param>
        protected void OnCreated(ref Entity en, ref Identity id) {

            var cached = EntityManagerFindExtension.CachedEntities;
            var buffered = EntityManagerFindExtension.BufferedEntity;
            
            // Add to cached and recycle if already existant
            if(!cached.ContainsKey(id.ID))
                cached.Add(id.ID, en);
            else {

                cached.Remove(id.ID);
                cached.Add(id.ID, en);
            }

            // Remove from buffered list
            if (buffered.ContainsKey(id.ID))
                buffered.Remove(id.ID);
        }
    }
}