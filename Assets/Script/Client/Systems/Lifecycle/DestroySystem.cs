using ParallelOrigin.Core.ECS.Components;
using Unity.Entities;

namespace Script.Client.Systems.Lifecycle {
    
    /// <summary>
    ///     A system processing <see cref="Destroy" /> and <see cref="DestroyAfter" /> components in order to destroy <see cref="Entity" />'s at the end of the frame.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class DestroySystem : SystemBase {
        
        private EndInitializationEntityCommandBufferSystem _beginBuffer;
        private EndSimulationEntityCommandBufferSystem _endBuffer;
        
        private EntityQuery _destructionQuery;
        
        protected override void OnCreate() {
            base.OnCreate();

            _destructionQuery = GetEntityQuery(typeof(Destroy));

            _beginBuffer = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            _endBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            
            // Counting ticks down
            var markCommandBuffer = _beginBuffer.CreateCommandBuffer();
            var destroyCommandBuffer = _endBuffer.CreateCommandBuffer();
            
            // Only those without destroy... otherwhise we might have problems with destruction
            Entities.ForEach((ref Entity entity, ref DestroyAfter after) => {
                    after.Ticks--;
                    if (after.Ticks <= 0) markCommandBuffer.AddComponent<Destroy>(entity);
            }).WithNone<Destroy>().Schedule();

            
            destroyCommandBuffer.DestroyEntitiesForEntityQuery(_destructionQuery);
        }
    }
}