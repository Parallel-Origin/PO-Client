using ParallelOrigin.Core.ECS.Components;
using Script.Client.Systems.Reactive;
using Unity.Burst;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(ReactiveSystem<Mesh, MeshAdded, MeshDestroyed>.State))]

namespace Script.Client.Systems.Reactive {
    
    [BurstCompile]
    public struct MeshAdded : IComponentData { }

    [BurstCompile]
    public struct MeshDestroyed : IComponentData { }

    /// <summary>
    ///     A reactive system that takes care of marking entities where a <see cref="Mesh" /> was added or removed recently
    /// </summary>
    public class MeshReactiveSystem : ReactiveSystem<Mesh, MeshAdded, MeshDestroyed> { }
}