using Script.Client.Systems.Reactive;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[assembly: RegisterGenericComponentType(typeof(ManagedReactiveSystem<GameObject, GameObjectAdded, GameObjectRemoved>.State))]

namespace Script.Client.Systems.Reactive {
    
    [BurstCompile]
    public struct GameObjectAdded : IComponentData { }

    [BurstCompile]
    public struct GameObjectRemoved : IComponentData { }

    /// <summary>
    ///     This system listens for <see cref="GameObject" /> getting attached to entities in order to trigger callbacks.
    /// </summary>
    public class GameObjectReactiveSystem : ManagedReactiveSystem<GameObject, GameObjectAdded, GameObjectRemoved> { }
}