using ParallelOrigin.Core.ECS.Components;
using Script.Client.Systems.Reactive;
using Unity.Burst;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(ReactiveSystem<Dirty, DirtyAdded, DirtyRemoved>.State))]

namespace Script.Client.Systems.Reactive {
    
    [BurstCompile]
    public struct DirtyAdded : IComponentData { }

    [BurstCompile]
    public struct DirtyRemoved : IComponentData { }

    /// <summary>
    ///     A reactive system informing via callbacks if a <see cref="Dirty" /> was added or removed at a <see cref="Entity" />
    /// </summary>
    public class DirtyReactiveSystem : ReactiveSystem<Dirty, DirtyAdded, DirtyRemoved> { }
}