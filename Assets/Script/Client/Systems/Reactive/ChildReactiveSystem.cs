using ParallelOrigin.Core.ECS.Components;
using Script.Client.Systems.Reactive;
using Unity.Burst;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(ReactiveSystem<Child, ChildAdded, ChildRemoved>.State))]
namespace Script.Client.Systems.Reactive {
    
    [BurstCompile]
    public struct ChildAdded : IComponentData { }

    [BurstCompile]
    public struct ChildRemoved : IComponentData { }

    /// <summary>
    ///     This system listens for <see cref="Child" /> getting attached to entities in order...
    /// </summary>
    public class ChildReactiveSystem : ReactiveSystem<Child, ChildAdded, ChildRemoved> { }
}