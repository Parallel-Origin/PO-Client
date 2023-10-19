using System.Collections.Generic;
using Unity.Entities;

namespace Script.Client.Systems.Reactive {

    /// <summary>
    /// An struct which stores a query and a type which gets destroyed after existing one frame in the ecs. 
    /// </summary>
    public struct OneFrame {
        public EntityQuery Query;
        public ComponentType Type;
    }
    
    /// <summary>
    /// An struct which stores a query and a type which gets destroyed after existing one frame in the ecs. 
    /// </summary>
    public struct BufferedOneFrame {
        public ComponentType[] Query;
        public ComponentType Type;
    }
    
    /// <summary>
    /// A system which tracks certain <see cref="ComponentTypes"/> to make sure they only stay one frame long.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [AlwaysUpdateSystem]
    public partial class OneFrameSystem : SystemBase{

        protected override void OnCreate() {
            base.OnCreate();
            
            AtFrameEndBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            Initialized = true;
        }

        protected override void OnUpdate() {

            var endCommandBuffer = AtFrameEndBuffer.CreateCommandBuffer();

            // Convert buffered queries to real ones and make use of them
            for (var index = 0; index < BufferedOneFrames.Count; index++) {

                var bufferedOneFrame = BufferedOneFrames[index];
                var query = GetEntityQuery(bufferedOneFrame.Query);
                
                if (Exists(bufferedOneFrame.Type, query)) continue;

                var oneFrame = new OneFrame {Query = query, Type = bufferedOneFrame.Type};
                OneFrames.Add(oneFrame);
            }

            BufferedOneFrames.Clear();
            
            // Find one frame components and remove them
            for (var index = 0; index < OneFrames.Count; index++) {

                var oneFrame = OneFrames[index];
                endCommandBuffer.RemoveComponentForEntityQuery(oneFrame.Query, oneFrame.Type);
            }
        }

        /// <summary>
        /// Registers a component that should only stay one frame long and a query used.
        /// </summary>
        /// <param name="query">A list of components that we use as a query for filtering our entities</param>
        /// <param name="oneFrameComponent">The component that stays one frame long</param>
        public void Register(ComponentType oneFrameComponent, params ComponentType[] query) {
            
            if (Initialized) {
                
                var entityQuery = GetEntityQuery(query);
                if (Exists(oneFrameComponent, entityQuery)) return;

                var oneFrame = new OneFrame {Query = entityQuery, Type = oneFrameComponent};
                OneFrames.Add(oneFrame);
            }
            else BufferedOneFrames.Add(new BufferedOneFrame{Query = query, Type = oneFrameComponent});

            Components.Add(oneFrameComponent);
        }

        /// <summary>
        /// Check if the given component was registered for exisitng one frame long with the certain query used to determine the entities.
        /// </summary>
        /// <param name="oneFrameComponent">The component that should only live one frame long</param>
        /// <param name="entityQuery">The query used to receive those components</param>
        /// <returns>True if it exists, otherwhise false</returns>
        protected bool Exists(ComponentType oneFrameComponent, EntityQuery entityQuery) {

            var componentExists = Components.Contains(oneFrameComponent);
            for (var index = 0; index < OneFrames.Count; index++) {

                var oneFrame = OneFrames[index];
                if (oneFrame.Query == entityQuery && componentExists) return true;
            }

            return false;
        }
        

        /// <summary>
        /// The buffer at the end of the frame which is used to remove the one frame component
        /// </summary>
        public EndSimulationEntityCommandBufferSystem AtFrameEndBuffer { get; set; }
        
        /// <summary>
        /// A list of components that are registered for staying one frame long
        /// </summary>
        public ISet<ComponentType> Components { get; } = new HashSet<ComponentType>();
        
        /// <summary>
        /// Raw, buffered queries to determine entities on which we wanna remove the one frame component.
        /// This is used if one system registers a one frame component but this system wasnt initialized yet. 
        /// </summary>
        public IList<BufferedOneFrame> BufferedOneFrames { get; } = new List<BufferedOneFrame>();
        
        /// <summary>
        /// Queries, used for destroying all one frame components after each other
        /// </summary>
        public IList<OneFrame> OneFrames { get; } = new List<OneFrame>();
        
        /// <summary>
        /// Shows if this system was initialized, used for buffering queries, otherwhise a exception may occur.
        /// </summary>
        public bool Initialized { get; set; }
    }
}