using Script.Client.Systems.Reactive;
using Unity.Entities;
using UnityEngine;
using Animation = ParallelOrigin.Core.ECS.Components.Animation;

namespace Script.Client.Systems.Graphical {
    
    /// <summary>
    ///     A system iterating over <see cref="GameObjectCreated" /> events in order to search the animator and attach it to the <see cref="Entity" /> itself
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class AddAnimatorSystem : SystemBase {
        protected override void OnUpdate() {
            
            // Check if the created gameobject has a animator attached, if so... add it to the entity itself
            Entities.ForEach((Entity entity, GameObject loaded) => {
                
                if (!EntityManager.HasComponent<Animation>(entity)) return;

                var animator = loaded.GetComponent<Animator>();
                if (animator) EntityManager.AddComponentObject(entity, animator);
            }).WithAll<GameObjectAdded>().WithStructuralChanges().WithoutBurst().Run();
        }
    }
}