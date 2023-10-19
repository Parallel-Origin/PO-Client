using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using Script.Extensions;
using Unity.Entities;

namespace Script.Client.Systems.Relation {
    
    /// <summary>
    /// A system which iterates over <see cref="Parent"/>'s to link their childs to the parent. 
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class ParentChildSystem : SystemBase {

        private EndInitializationEntityCommandBufferSystem _commandBufferSystem;

        protected override void OnCreate() {
            base.OnCreate();
            _commandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {

            var ecb = _commandBufferSystem.CreateCommandBuffer();
            var mapping = EntityManagerFindExtension.CachedEntities;
            
            // Loop over all parents to assign link the childs to the parents each frame
            Entities.ForEach((in Entity entity, in Parent parent) => {

                for (var index = 0; index < parent.Children.Length; index++) {

                    var childRef = parent.Children[index];
                    var childEntity = childRef.Resolve(ref mapping);

                    // Might throw errors otherwhise
                    if (childEntity == Entity.Null) return;
                    if(HasComponent<Destroy>(childEntity)) continue;

                    // Create entity reference to parent and give it to the child. 
                    var parentRef = new Child { Parent = new EntityLink(in entity) };
                    if (HasComponent<Child>(childEntity)) 
                        ecb.SetComponent(childEntity, parentRef);
                    else ecb.AddComponent(childEntity, parentRef);
                }
            }).WithNone<Destroy>().Schedule();
            _commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}