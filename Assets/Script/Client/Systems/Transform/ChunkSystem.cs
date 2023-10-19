using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Transform;
using Script.Client.Mono.Map;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Grid = ParallelOrigin.Core.Base.Classes.Grid;
using Vector2dExtensions = Script.Extensions.Vector2dExtensions;

namespace Script.Client.Systems.Transform {
    /// <summary>
    ///     A system which calculates the chunks for each entity, aswell as destroys entities outside the visual area.
    /// </summary>
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public partial class ChunkSystem : SystemBase {
        
        private MovementArea _movementArea;

        protected override void OnCreate() {
            base.OnCreate();

            ServiceLocator.Wait<MovementArea>(o => _movementArea = (MovementArea) o);
        }
        
        protected override void OnUpdate() {

            //Calculate new Chunk Position
            Entities.ForEach((ref LocalTransform transform) => {
                
                if (!transform.Chunk.X.Equals(0) || !transform.Chunk.Y.Equals(0)) return;

                //calculate new pos
                var tile = Vector2dExtensions.CoordinateToTileId(transform.Pos, 14);
                transform.Chunk = new Grid((ushort)tile.X, (ushort)tile.Y);
            }).Schedule();
            
            // Hides entities which are outside the movement area for not showing them to the user.
            Entities.ForEach((ref Entity entity, ref LocalTransform lc, in MeshRenderer meshRenderer) => {
                
                var vec2 = (Mapbox.Utils.Vector2d)lc.Pos;
                meshRenderer.enabled = _movementArea.InArea(vec2);
            }).WithNone<Structure>().WithoutBurst().Run();
            
            // Hides entities which are outside the movement area for not showing them to the user.
            Entities.ForEach((ref Entity entity, ref LocalTransform lc, in SkinnedMeshRenderer skinnedMeshRenderer) => {
                
                var vec2 = (Mapbox.Utils.Vector2d)lc.Pos;
                skinnedMeshRenderer.enabled = _movementArea.InArea(vec2);
            }).WithoutBurst().Run();
        }
    }
}