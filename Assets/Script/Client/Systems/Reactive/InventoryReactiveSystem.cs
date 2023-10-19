using ParallelOrigin.Core.ECS.Components.Items;
using Script.Client.Systems.Reactive;
using Unity.Burst;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(ReactiveSystem<AddedToInventory, AddedToInventoryAdded, AddedToInventoryRemoved>.State))]
[assembly: RegisterGenericComponentType(typeof(ReactiveSystem<RemovedFromInventory, RemovedFromInventoryAdded, RemovedFromInventoryRemoved>.State))]

namespace Script.Client.Systems.Reactive {
    
    [BurstCompile]
    public struct AddedToInventoryAdded : IComponentData { }

    [BurstCompile]
    public struct AddedToInventoryRemoved : IComponentData { }

    /// <summary>
    /// This system listens for <see cref="AddedToInventory" /> getting attached to entities in order...
    /// </summary>
    public class AddedToInventoryReactiveSystem : ReactiveSystem<AddedToInventory, AddedToInventoryAdded, AddedToInventoryRemoved> { }
    
        
    [BurstCompile]
    public struct RemovedFromInventoryAdded : IComponentData { }

    [BurstCompile]
    public struct RemovedFromInventoryRemoved : IComponentData { }

    /// <summary>
    /// This system listens for <see cref="AddedToInventory" /> getting attached to entities in order...
    /// </summary>
    public class RemovedFromInventoryReactiveSystem : ReactiveSystem<RemovedFromInventory, RemovedFromInventoryAdded, RemovedFromInventoryRemoved> { }
}