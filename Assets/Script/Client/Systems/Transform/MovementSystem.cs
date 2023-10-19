using Mapbox.Unity.Map;
using ParallelOrigin.Core.Base.Classes;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Transform;
using Script.Extensions;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Script.Client.Systems.Transform {
    
    /// <summary>
    /// A system taking care of the movement of entities
    /// </summary>
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public partial class MovementSystem : SystemBase {
        
        private EndInitializationEntityCommandBufferSystem _commandBufferSystem;
        private AbstractMap _abstractMap;

        protected override void OnCreate() {
            base.OnCreate();

            _commandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            ServiceLocator.Wait<AbstractMap>(o => _abstractMap = (AbstractMap) o);
        }

        protected override void OnUpdate() {
            
            var ecb = _commandBufferSystem.CreateCommandBuffer();
            
            // Required for GeoLocation converting and speed
            var map = _abstractMap;
            var refPoint = (Vector2d)map.CenterMercator;
            var scale = map.WorldRelativeScale;
            var time = Time.DeltaTime;

            // Add a location component to entities with network location only to ensure they show up ingame
            Entities.ForEach((ref Entity entity, ref NetworkTransform nlc) => {
                
                ecb.AddComponent(entity, new LocalTransform{Pos = nlc.Pos});
            }).WithNone<LocalTransform>().Schedule();
            
            // Teleportion to network location for entities without movement
            Entities.ForEach((ref NetworkTransform nlc, ref LocalTransform lc) => {

                // In case that lc.pos is zero, we teleport it to the network location
                var zero = Vector2d.Zero;
                if (lc.Pos.X.Equals(zero.X) && lc.Pos.Y.Equals(zero.Y)) lc.Pos = nlc.Pos;
            }).WithNone<Movement>().Schedule();
            
            // Interpolation between network location and real location
            Entities.ForEach((ref NetworkTransform nlc, ref LocalTransform lc, ref Movement mvto) => {

                // In case that lc.pos is zero, we teleport it to the network location
                var zero = Vector2d.Zero;
                if (lc.Pos.X.Equals(zero.X) && lc.Pos.Y.Equals(zero.Y)) lc.Pos = nlc.Pos;

                var speed = time * mvto.Speed;
                lc.Pos = Vector2d.MoveTowards(lc.Pos, nlc.Pos, speed);
            }).Schedule();
            
            // LatLng - Position to UnityPos converting
            Entities.ForEach((ref LocalTransform lc, in GameObject go) => {
                
                go.transform.position = lc.Pos.AsUnityPosition(refPoint, scale);
            }).WithNone<Item>().WithoutBurst().Run();
            
            // LatLng - Position to UnityPos converting for items, require a different height handling for showing up above the tiles
            Entities.ForEach((ref LocalTransform lc, in GameObject go) => {
                
                var pos = lc.Pos.AsUnityPosition(refPoint, scale);
                pos.y = 1;
                go.transform.position = pos;
            }).WithAll<Item>().WithoutBurst().Run();
        }
    }
}