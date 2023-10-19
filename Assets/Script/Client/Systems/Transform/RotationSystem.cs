using ParallelOrigin.Core.ECS.Components.Transform;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Script.Client.Systems.Transform {
    
    /// <summary>
    ///     This system manages the rotation of the player to a target directon from the <see cref="RotateTo" />
    /// </summary>
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public partial class RotationSystem : SystemBase {
        
        private EndInitializationEntityCommandBufferSystem _commandBufferSystem;

        protected override void OnCreate() {
            base.OnCreate();
            
            _commandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            
            var ecb = _commandBufferSystem.CreateCommandBuffer();
            var time = Time.DeltaTime;

            // Add a rotation component to entities with network rotation only to ensure they rotate ingame
            Entities.ForEach((ref Entity entity, ref NetworkRotation netRot) => {
                
                ecb.AddComponent(entity, new Rotation{Value = netRot.Value});
            }).WithNone<Rotation>().Schedule();
            
            // Interpolating the rotation towards the network rotation
            Entities.ForEach((ref NetworkRotation netRot, ref Rotation rotation) =>
            {
                var speed = time * 3.5f;
                rotation.Value = math.nlerp(rotation.Value, netRot.Value, speed);
            }).Schedule();

            // Apply component rotation to the gameobject itself
            Entities.ForEach((ref Rotation rotation, in GameObject go ) => {
                go.transform.rotation = rotation.Value;
            }).WithoutBurst().Run();
        }
    }
}