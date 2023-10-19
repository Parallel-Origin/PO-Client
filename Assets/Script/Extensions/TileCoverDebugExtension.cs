using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;
using Tile = Script.Client.Mono.Map.Tile;

namespace Script.Extensions {

    /// <summary>
    ///     A extension that is used to debug <see cref="TileCover" />'s used by mapbox to represent the tiles of a world map.
    /// </summary>
    public static class TileCoverDebugExtension {

        /// <summary>
        ///     Returns the size of a tile.
        /// </summary>
        /// <param name="z">The zoom level we wanna get the tile size of</param>
        /// <returns>The size of a tile ( unity size ) at the given zoom level</returns>
        public static float GetTileSize(AbstractMap map, int z) {

            var latLngPosition = Vector2.zero.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);

            var tile = TileCover.CoordinateToTileId(latLngPosition, z);
            var newTile = new UnwrappedTileId(z, tile.X + 1, tile.Y + 1);

            var oldPos = TileCover.TileIdToCoordinate(tile);
            var newPos = TileCover.TileIdToCoordinate(newTile);

            var oldPosition = new Vector2d(oldPos.y, oldPos.x).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
            var newPosition = new Vector2d(newPos.y, newPos.x).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);

            return newPosition.x - oldPosition.x;
        }

        /// <summary>
        ///     Used for debugging the tiles by spawning them visually into the engine as graphical tiles.
        /// </summary>
        /// <param name="tile">The tile we use as a center</param>
        public static void DebugTiles(AbstractMap map, UnwrappedTileId tile) {

            var startX = tile.X;
            var startY = tile.Y;
            var tileSize = GetTileSize(map, tile.Z);

            // Cubes makieren zenter der tiles.
            for (var index = startX - 2; index <= startX + 2; index++) {
                for (var sindex = startY - 2; sindex <= startY + 2; sindex++) {

                    var newTile = new UnwrappedTileId(Tile.ZoomForTileCalculation, index, sindex);
                    var pos = TileCover.TileIdToCoordinate(newTile);
                    var unityPos = new Vector2d(pos.y, pos.x).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);

                    var exactPosition = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    exactPosition.transform.position = unityPos;
                    exactPosition.transform.localScale = new Vector3(10, 50, 10);
                    exactPosition.layer = LayerMask.NameToLayer("Environment");
                    exactPosition.name = "Exact : " + newTile.X + " " + newTile.Y + " " + newTile.Z;

                    var area = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    area.transform.position = new Vector3(unityPos.x + tileSize / 2, unityPos.y, unityPos.z - tileSize / 2);
                    area.transform.localScale = new Vector3(tileSize - 5, 1, tileSize - 5);
                    area.layer = LayerMask.NameToLayer("Environment");
                    area.name = newTile.X + " " + newTile.Y + " " + newTile.Z;
                }
            }
        }
    }
}