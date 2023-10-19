using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Script.Client.Systems.Reactive {

    /// <summary>
    /// A delegate being invoked once the <see cref="ReactiveSystem{TComponent,TAdded,TRemoved}"/> added an component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void OnAdded<T>(ref Entity entity, ref T addedComponent) where T : struct, IComponentData;
    
    /// <summary>
    /// A delegate being invoked once the <see cref="ReactiveSystem{TComponent,TAdded,TRemoved}"/> removed an component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void OnRemoved<T>(ref Entity entity, in T removedComponent) where T : struct, IComponentData;
    
    /// <summary>
    /// A delegate being invoked once the <see cref="ReactiveSystem{TComponent,TAdded,TRemoved}"/> added an component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void OnAddedClass<T>(ref Entity entity, ref T addedComponent) where T : class;
    
    /// <summary>
    /// A delegate being invoked once the <see cref="ReactiveSystem{TComponent,TAdded,TRemoved}"/> removed an component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void OnRemovedClass<T>(ref Entity entity, in T removedComponent) where T : class;
    
    
    /// <summary>
    ///     There no callbacks or listeners for added/removed components on <see cref="Entity" />'s
    ///     Thats where this system comes in using <see cref="ISystemStateComponentData" /> for simulating those callbacks inside the ecs.
    ///     <typeparam name="Component">The component we wanna listen to</typeparam>
    ///     <typeparam name="Added">The component which indicates that our component has been added, gets attached for one frame to the entity</typeparam>
    ///     <typeparam name="Removed">The component which indicates that our component was removed, gets attached for one frame to the entity</typeparam>
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public abstract partial class ReactiveSystem<TComponent, TAdded, TRemoved> : SystemBase where TComponent : unmanaged, IComponentData where TAdded : struct, IComponentData where TRemoved : struct,IComponentData {
        
        private EndInitializationEntityCommandBufferSystem _atFrameStartBuffer;

        public OnAdded<TComponent> OnAdded;
        public OnRemoved<TComponent> OnRemoved;
        
        protected EntityQuery NewEntities;
        protected EntityQuery EntitiesWithAdded;
        protected EntityQuery EntitiesWithStateOnly;
        protected EntityQuery ToRemoveEntities;
        
        protected EntityQuery CopyEntities;

        protected override void OnCreate() {
            base.OnCreate();

            _atFrameStartBuffer = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();

            OnAdded += (ref Entity en, ref TComponent component) => { };
            OnRemoved += (ref Entity en, in TComponent component) => { };
            
            // Query to get all newly created entities, without being marked as added
            NewEntities = GetEntityQuery(new EntityQueryDesc {
                All = new[] {ComponentType.ReadOnly<TComponent>()}, 
                None = new[] {ComponentType.ReadOnly<TAdded>(), ComponentType.ReadOnly<State>()}
            });
            
            // Query of all entities which where added this frame
            EntitiesWithAdded = GetEntityQuery(new EntityQueryDesc {
                All = new[] { ComponentType.ReadOnly<State>(), ComponentType.ReadOnly<TAdded>()}, 
                None = new[] {ComponentType.ReadOnly<TRemoved>()}
            });
            
            // Query of all entities which where added this frame
            EntitiesWithStateOnly = GetEntityQuery(new EntityQueryDesc {
                All = new[] { ComponentType.ReadOnly<State>()}, 
                None = new[] {ComponentType.ReadOnly<TComponent>(), ComponentType.ReadOnly<TAdded>(), ComponentType.ReadOnly<TRemoved>()}
            });

            // Query of all entities which where removed this frame
            ToRemoveEntities = GetEntityQuery(new EntityQueryDesc {
                All = new[] {ComponentType.ReadOnly<State>(), ComponentType.ReadOnly<TRemoved>()}, 
                None = new[] {ComponentType.ReadOnly<TComponent>(), ComponentType.ReadOnly<TAdded>()}
            });
            
            // Query entities which require a copy of the state each frame
            CopyEntities = GetEntityQuery(new EntityQueryDesc {
                All = new[] {ComponentType.ReadOnly<TComponent>(), ComponentType.ReadWrite<State>()}
            });

            JobEntityBatchExtensions.EarlyJobInit<AddedJob>();
            JobEntityBatchExtensions.EarlyJobInit<RemovedJob>();
            JobEntityBatchExtensions.EarlyJobInit<RemoveAddedJob>();
            JobEntityBatchExtensions.EarlyJobInit<RemoveRemovedJob>();
            JobEntityBatchExtensions.EarlyJobInit<CopyJob>();
        }

        protected override void OnUpdate() {
            
            var ecb = _atFrameStartBuffer.CreateCommandBuffer();
            var ecbParallel = _atFrameStartBuffer.CreateCommandBuffer().AsParallelWriter();
 
            var addedEntityCount = NewEntities.CalculateEntityCount();
            var removedEntityCound = EntitiesWithStateOnly.CalculateEntityCount();
            
            var added = new NativeList<Transmution>(addedEntityCount, Allocator.TempJob);
            var removed = new NativeList<Transmution>(removedEntityCound, Allocator.TempJob);
            
            // Add the added component job
            var addedJob = new AddedJob{
                Ecb = ecbParallel,
                EntityHandle = GetEntityTypeHandle(),
                ComponentHandle = GetComponentTypeHandle<TComponent>(true),
                Added = added.AsParallelWriter()
            };
            addedJob.ScheduleParallel(NewEntities, Dependency).Complete();
            
            // Call the reactor 
            for (var index = 0; index < added.Length; index++) {

                var addedTransmution = added[index];
                OnAdded(ref addedTransmution.Entity, ref addedTransmution.Component);
                ecb.SetComponent(addedTransmution.Entity, addedTransmution.Component);
            }
            
            // Add the remove component job
            var removeJob = new RemovedJob{
                Ecb = ecbParallel,
                EntityHandle = GetEntityTypeHandle(),
                ComponentHandle = GetComponentTypeHandle<State>(true),
                Reactors = removed.AsParallelWriter()
            };
            removeJob.ScheduleParallel(EntitiesWithStateOnly, Dependency).Complete();
            
            // Call the reactor to inform about removed
            for (var index = 0; index < removed.Length; index++) {

                var removedTransmution = removed[index];
                OnRemoved(ref removedTransmution.Entity, in removedTransmution.Component);
            }
            
            // Remove the added component
            var removeAddedJob = new RemoveAddedJob{
                Ecb = ecbParallel,
                EntityHandle = GetEntityTypeHandle(),
            };
            Dependency = removeAddedJob.ScheduleParallel(EntitiesWithAdded, Dependency);

            // Remove the removed component
            var removeRemovedJob = new RemoveRemovedJob{
                Ecb = ecbParallel,
                EntityHandle = GetEntityTypeHandle(),
            };
            Dependency = removeRemovedJob.ScheduleParallel(ToRemoveEntities, Dependency);
            
            // Create job to copy the TComponent into the state 
            var copyJob = new CopyJob {
                ComponentTypeHandle = GetComponentTypeHandle<TComponent>(true),
                StateTypeHandle = GetComponentTypeHandle<State>()
            };
            Dependency = copyJob.ScheduleParallel(CopyEntities, Dependency);

            // Dispose and add make ecb concurrent
            _atFrameStartBuffer.AddJobHandleForProducer(Dependency);
            added.Dispose();
            removed.Dispose();
        }

        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        [BurstCompile]
        private struct AddedJob : IJobEntityBatch {

            public EntityCommandBuffer.ParallelWriter Ecb;

            [ReadOnly] public EntityTypeHandle EntityHandle;
            [ReadOnly] public ComponentTypeHandle<TComponent> ComponentHandle;
            
            public NativeList<Transmution>.ParallelWriter Added;

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);
                var componentArray = chunk.GetNativeArray(ComponentHandle);
  
                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    var component = componentArray[i];
                    
                    var transmution = new Transmution {Entity = entity, Component = component};
                    Added.AddNoResize(transmution);

                    Ecb.AddComponent(chunkIndex, entity, new TAdded());
                    Ecb.AddComponent(chunkIndex, entity, new State {Component = component});
                }
            }
        }

        /// <summary>
        /// A job which removes the <see cref="TAdded"/> from the entity because its a one frame marker
        /// </summary>
        [BurstCompile]
        private struct RemoveAddedJob : IJobEntityBatch {

            public EntityCommandBuffer.ParallelWriter Ecb;
            
            [ReadOnly] public EntityTypeHandle EntityHandle;

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);

                // Remove the added
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    Ecb.RemoveComponent<TAdded>(chunkIndex, entity);
                }
            }
        }
        
        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        [BurstCompile]
        private struct RemovedJob : IJobEntityBatch {
            
            public EntityCommandBuffer.ParallelWriter Ecb;
            
            [ReadOnly] public EntityTypeHandle EntityHandle; 
            [ReadOnly] public ComponentTypeHandle<State> ComponentHandle;

            public NativeList<Transmution>.ParallelWriter Reactors;

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);
                var componentArray = chunk.GetNativeArray(ComponentHandle);
     
                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    var state = componentArray[i];
                    var oldState = state.Component;
                    
                    var transmution = new Transmution {Entity = entity, Component = oldState};
                    Reactors.AddNoResize(transmution);

                    Ecb.AddComponent(chunkIndex, entity, new TRemoved());
                }
            }
        }
        
        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        [BurstCompile]
        private struct RemoveRemovedJob : IJobEntityBatch {
            
            public EntityCommandBuffer.ParallelWriter Ecb;
            
            [ReadOnly] public EntityTypeHandle EntityHandle; 

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);

                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    Ecb.RemoveComponent<TRemoved>(chunkIndex, entity);
                    Ecb.RemoveComponent<State>(chunkIndex, entity);
                }
            }
        }
        
        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        [BurstCompile]
        private struct CopyJob : IJobEntityBatch {
            
            [ReadOnly] public ComponentTypeHandle<TComponent> ComponentTypeHandle;
            public ComponentTypeHandle<State> StateTypeHandle;

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var componentArray = chunk.GetNativeArray(ComponentTypeHandle);
                var stateArray = chunk.GetNativeArray(StateTypeHandle);
                
                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var component = componentArray[i];
                    stateArray[i] = new State {Component = component};
                }
            }
        }

        /// <summary>
        /// The internal state to mark entities
        /// </summary>
        [BurstCompile]
        public struct State : ISystemStateComponentData {
            public TComponent Component;
        }

        /// <summary>
        /// An struct which represents an transmution of the <see cref="TComponent"/> which was either added or removed. 
        /// </summary>
        public struct Transmution {
            public Entity Entity;
            public TComponent Component;
        }
    }
    
     /// <summary>
    ///     There no callbacks or listeners for added/removed components on <see cref="Entity" />'s
    ///     Thats where this system comes in using <see cref="ISystemStateComponentData" /> for simulating those callbacks inside the ecs.
    ///     <typeparam name="Component">The component we wanna listen to</typeparam>
    ///     <typeparam name="Added">The component which indicates that our component has been added, gets attached for one frame to the entity</typeparam>
    ///     <typeparam name="Removed">The component which indicates that our component was removed, gets attached for one frame to the entity</typeparam>
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public abstract partial class ManagedReactiveSystem<TComponent, TAdded, TRemoved> : SystemBase where TComponent : class where TAdded : struct, IComponentData where TRemoved : struct, IComponentData {
         
        private EndInitializationEntityCommandBufferSystem _atFrameStartBuffer;

        public OnAddedClass<TComponent> OnAdded;
        public OnRemovedClass<TComponent> OnRemoved;

        protected EntityQuery NewEntities;
        protected EntityQuery EntitiesWithAdded;
        protected EntityQuery EntitiesWithStateOnly;
        protected EntityQuery ToRemoveEntities;
        protected EntityQuery CopyEntities;

        protected override void OnCreate() {
            base.OnCreate();

            _atFrameStartBuffer = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();

            OnAdded += (ref Entity en, ref TComponent component) => { };
            OnRemoved += (ref Entity en, in TComponent component) => { };
            
            // Query to get all newly created entities, without being marked as added
            NewEntities = GetEntityQuery(new EntityQueryDesc {
                All = new[] {ComponentType.ReadOnly<TComponent>()}, 
                None = new[] {ComponentType.ReadOnly<TAdded>(), ComponentType.ReadOnly<State>()}
            });
            
            // Query of all entities which where added this frame
            EntitiesWithAdded = GetEntityQuery(new EntityQueryDesc {
                All = new[] { ComponentType.ReadOnly<State>(), ComponentType.ReadOnly<TAdded>()}, 
                None = new[] {ComponentType.ReadOnly<TRemoved>()}
            });
            
            // Query of all entities which where added this frame
            EntitiesWithStateOnly = GetEntityQuery(new EntityQueryDesc {
                All = new[] { ComponentType.ReadOnly<State>()}, 
                None = new[] {ComponentType.ReadOnly<TComponent>(), ComponentType.ReadOnly<TAdded>(), ComponentType.ReadOnly<TRemoved>()}
            });

            // Query of all entities which where removed this frame
            ToRemoveEntities = GetEntityQuery(new EntityQueryDesc {
                All = new[] {ComponentType.ReadOnly<State>(), ComponentType.ReadOnly<TRemoved>()}, 
                None = new[] {ComponentType.ReadOnly<TComponent>(), ComponentType.ReadOnly<TAdded>()}
            });
            
            // Query entities which require a copy of the state each frame
            CopyEntities = GetEntityQuery(new EntityQueryDesc {
                All = new[] {ComponentType.ReadOnly<TComponent>(), ComponentType.ReadWrite<State>()}
            });
            
            JobEntityBatchExtensions.EarlyJobInit<AddedJob>();
            JobEntityBatchExtensions.EarlyJobInit<RemovedJob>();
            JobEntityBatchExtensions.EarlyJobInit<RemoveAddedJob>();
            JobEntityBatchExtensions.EarlyJobInit<RemoveRemovedJob>();
            JobEntityBatchExtensions.EarlyJobInit<CopyJob>();
        }

        protected override void OnUpdate() {
            
            var startCommandBuffer = _atFrameStartBuffer.CreateCommandBuffer();
            
            // Add the added component job
            var addedJob = new AddedJob{
                EntityManager = EntityManager,
                Ecb = startCommandBuffer,
                EntityHandle = GetEntityTypeHandle(),
                ComponentHandle = EntityManager.GetComponentTypeHandle<TComponent>(true),
                Reactor = OnAdded
            };
            JobEntityBatchExtensions.RunWithoutJobs(ref addedJob, NewEntities); 
            
            // Add the remove component job
            var removeJob = new RemovedJob{
                EntityManager = EntityManager,
                Ecb = startCommandBuffer,
                EntityHandle = GetEntityTypeHandle(),
                ComponentHandle = EntityManager.GetComponentTypeHandle<State>(true),
                Reactor = OnRemoved
            };
            JobEntityBatchExtensions.RunWithoutJobs(ref removeJob, EntitiesWithStateOnly);
            
            // Remove the added component
            var removeAddedJob = new RemoveAddedJob{
                Ecb = startCommandBuffer,
                EntityHandle = GetEntityTypeHandle(),
            };
            JobEntityBatchExtensions.RunWithoutJobs(ref removeAddedJob, EntitiesWithAdded);

            // Remove the removed component
            var removeRemovedJob = new RemoveRemovedJob{
                Ecb = startCommandBuffer,
                EntityHandle = GetEntityTypeHandle(),
            };
            JobEntityBatchExtensions.RunWithoutJobs(ref removeRemovedJob, ToRemoveEntities);

            // Create job to copy the TComponent into the state 
            var copyJob = new CopyJob {
                EntityManager = EntityManager,
                ComponentTypeHandle = EntityManager.GetComponentTypeHandle<TComponent>(true),
                StateTypeHandle = EntityManager.GetComponentTypeHandle<State>(false)
            };
            JobEntityBatchExtensions.RunWithoutJobs(ref copyJob, CopyEntities);
        }

        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        private struct AddedJob : IJobEntityBatch {

            public EntityManager EntityManager;
            public EntityCommandBuffer Ecb;

            [ReadOnly] public EntityTypeHandle EntityHandle;
            [ReadOnly] public ComponentTypeHandle<TComponent> ComponentHandle;

            public OnAddedClass<TComponent> Reactor;

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {
                
                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);
                var componentArray = chunk.GetManagedComponentAccessor(ComponentHandle, EntityManager);
                
                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    var component = componentArray[i];
                    
                    Reactor(ref entity, ref component);

                    Ecb.AddComponent(entity, new TAdded());
                    Ecb.AddComponent(entity, new State {Component = component});
                }
            }
        }
        
        /// <summary>
        /// A job which removes the <see cref="TAdded"/> from the entity because its a one frame marker
        /// </summary>
        private struct RemoveAddedJob : IJobEntityBatch {
            
            public EntityCommandBuffer Ecb;
            [ReadOnly] public EntityTypeHandle EntityHandle;
            
            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);

                // Remove the added
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    Ecb.RemoveComponent<TAdded>(entity);
                }
            }
        }

        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        private struct RemovedJob : IJobEntityBatch {

            public EntityManager EntityManager;
            public EntityCommandBuffer Ecb;
            
            [ReadOnly] public EntityTypeHandle EntityHandle; 
            [ReadOnly] public ComponentTypeHandle<State> ComponentHandle;

            public OnRemovedClass<TComponent> Reactor;
            
            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);
                var componentArray = chunk.GetManagedComponentAccessor(ComponentHandle, EntityManager);

                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    var state = componentArray[i];
                    var oldState = state.Component;

                    Reactor(ref entity, in oldState);

                    Ecb.AddComponent(entity, new TRemoved());
                }
            }
        }
        
         /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
         private struct RemoveRemovedJob : IJobEntityBatch {
            
            public EntityCommandBuffer Ecb;
            [ReadOnly] public EntityTypeHandle EntityHandle; 

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var entityArray = chunk.GetNativeArray(EntityHandle);

                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var entity = entityArray[i];
                    Ecb.RemoveComponent<TRemoved>(entity);
                    Ecb.RemoveComponent<State>(entity);
                }
            }
        }

        /// <summary>
        /// A job which runs asynchron and copies the <see cref="TComponent"/> into the <see cref="State"/> in an fast and efficient, generic way. 
        /// </summary>
        private struct CopyJob : IJobEntityBatch {

            public EntityManager EntityManager;
            
            [ReadOnly] public ComponentTypeHandle<TComponent> ComponentTypeHandle;
            public ComponentTypeHandle<State> StateTypeHandle;

            public void Execute(ArchetypeChunk chunk, int chunkIndex) {

                // Get original component and the state array
                var componentArray = chunk.GetManagedComponentAccessor(ComponentTypeHandle, EntityManager);
                var stateArray = chunk.GetManagedComponentAccessor(StateTypeHandle, EntityManager);
                
                // Copy the component into the state
                for (var i = 0; i < chunk.Count; i++) {

                    var component = componentArray[i];
                    stateArray[i].Component = component;
                }
            }
        }

        /// <summary>
        /// The internal state to mark entities
        /// </summary>
        public class State : ISystemStateComponentData {
            public TComponent Component;
        }
     }
}