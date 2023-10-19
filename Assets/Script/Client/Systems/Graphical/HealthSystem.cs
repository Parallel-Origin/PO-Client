using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Combat;
using Script.Client.Mono.MVVM.Combat;
using Unity.Entities;
using UnityEngine;

namespace Script.Client.Systems.Graphical {
    
    /// <summary>
    ///     A system which calculates the health and reacts to damage/healing events for updating the visual representation of a entity.
    /// </summary>
    public partial class HealthSystem : SystemBase {
        
        protected override void OnUpdate() {

            // Updating healthbar for visual entity
            Entities.ForEach((ref Health hc, in GameObject go) => {
                
                // Updating visual health
                var health = go.GetComponent<HealthViewModel>();
                health.SetHealth(hc.CurrentHealth, hc.MaxHealth);
                
            }).WithoutBurst().Run();

            // Checking health and removing/hiding entity if dead
            Entities.ForEach((ref Entity entity, ref Health hc, in GameObject go, in SkinnedMeshRenderer smr) => {
                
                if (hc.CurrentHealth > 0) return;
                if (hc.DestroyOnDeath) EntityManager.AddComponentData(entity, new Destroy());
                else smr.enabled = false;
                
            }).WithStructuralChanges().WithoutBurst().Run();
            
            // Checking health and removing/hiding entity if dead
            Entities.ForEach((ref Entity entity, ref Health hc, in GameObject go, in MeshRenderer smr) => {
                
                if (hc.CurrentHealth > 0) return;
                if (hc.DestroyOnDeath) EntityManager.AddComponentData(entity, new Destroy());
                else smr.enabled = false;
                
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}