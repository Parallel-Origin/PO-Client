using Script.Client.Systems.Reactive;
using Unity.Entities;
using UnityEngine;

namespace Script.Client.Systems.Graphical {
    
    /// <summary>
    /// A system iterating over <see cref="GameObjectCreated" /> events in order to search the renderer and attach it to the <see cref="Entity" />
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class AddRendererSystem : SystemBase {
        protected override void OnUpdate() {
            
            // Searches for a renderer and attaches it directly to the entity
            Entities.ForEach((Entity entity, GameObject loaded) => {
                
                var renderer = loaded.GetComponentInChildren<Renderer>();
                var canvasRenderer = loaded.GetComponentInChildren<Canvas>();
                
                if (renderer) EntityManager.AddComponentObject(entity, renderer);
                if(canvasRenderer) EntityManager.AddComponentObject(entity, canvasRenderer);
            }).WithAll<GameObjectAdded>().WithStructuralChanges().WithoutBurst().Run();
        }
    }
}