using ParallelOrigin.Core.ECS.Components.Items;
using Script.Client.Systems.Reactive;
using Unity.Entities;

namespace Script.Client.Systems {
    
    /// <summary>
    /// A system which simply takes care of removing <see cref="AddedToInventory"/> and <see cref="RemovedFromInventory"/> after exactly one frame. 
    /// </summary>
    public partial class InventorySystem : SystemBase {

        private OneFrameSystem _oneFrameSystem;
 
        protected override void OnCreate() {
            base.OnCreate();

            _oneFrameSystem = World.GetOrCreateSystem<OneFrameSystem>();
         
            _oneFrameSystem.Register(typeof(AddedToInventory));
            _oneFrameSystem.Register(typeof(RemovedFromInventory));
        }

        protected override void OnUpdate() {

            /*
            Entities.ForEach((ref Entity entity, ref Item item) => {
                
                Debug.Log("TEST : "+item.amount);
            }).WithoutBurst().Run();*/
            
            /*
            // Diposes the list in the marker
            Entities.ForEach((ref Entity en, ref AddedToInventoryRemoved rm) => {

                if (HasComponent<AddedToInventoryReactiveSystem.State>(en)) {

                    var state = GetComponent<AddedToInventoryReactiveSystem.State>(en);
                    state.component.added.Dispose();
                }
            }).Schedule();
            
            // Diposes the list in the marker
            Entities.ForEach((ref Entity en, ref RemovedFromInventoryRemoved rm) => {

                if (HasComponent<RemovedFromInventoryReactiveSystem.State>(en)) {

                    var state = GetComponent<RemovedFromInventoryReactiveSystem.State>(en);
                    state.component.removed.Dispose();
                }
            }).Schedule();*/
        }
    }
}