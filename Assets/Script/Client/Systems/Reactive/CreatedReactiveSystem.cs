using ParallelOrigin.Core.ECS.Components;
using Script.Client.Systems.Reactive;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(ReactiveSystem<Identity, Created, Destroyed>.State))]

namespace Script.Client.Systems.Reactive {
    
    /// <summary>
    ///     There no callbacks or listeners for added/removed components on <see cref="Entity" />'s
    ///     Thats where this system comes in using <see cref="ISystemStateComponentData" /> for simulating those callbacks inside the ecs.
    /// </summary>
    public class CreatedReactiveSystem : ReactiveSystem<Identity, Created, Destroyed> { }
}