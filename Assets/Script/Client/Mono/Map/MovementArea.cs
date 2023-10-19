using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Script.Client.Mono.Map {
    /// <summary>
    ///     This class is used to represent the actuall movement area of the player... it contains several methods to check and manage this area.
    /// </summary>
    public class MovementArea : MonoBehaviour {
        
        [SerializeField] private AbstractMap map;

        [SerializeField] private Transform movementArea;
        [SerializeField] private Transform rightBottom;
        [SerializeField] private Transform rightTop;
        [SerializeField] private Transform leftBottom;
        [SerializeField] private Transform leftTop;

        [SerializeField] private Vector2d latLngPositionLeftTop;
        [SerializeField] private Vector2d latLngPositionRightBottom;

        [SerializeField] private Vector3 positionLeftTop;
        [SerializeField] private Vector3 positionRightBottom;

        private void Awake() {
            ServiceLocator.Register(this);
            map.OnInitialized += SetCorners;
        }


        /////////////////////////////////
        /// Main methods
        /////////////////////////////////


        /// <summary>
        ///     This method calculates a new position for the movement area based on higher zoom tiles
        ///     and changes its location aswell as updates the internal corner positions
        /// </summary>
        /// <param name="position">The GeoLocation where we wanna center this area around</param>
        public void Reposition(Vector2d position) {
            
            var tile = TileCover.CoordinateToTileId(position, 14);
            var tileLatLngPosition = TileCover.TileIdToCoordinate(tile);

            SetPosition(tileLatLngPosition);
            SetCorners();
        }

        /// <summary>
        ///     Gives the corners of the movement area a fitting lat lng position.
        ///     This is needed to check if a certain entity position is within those bounds.
        /// </summary>
        public void SetCorners() {
            var size = GetSize();

            // Calculation new geo locations for the corners of the area
            var movementAreaPosition = new float3(movementArea.position.x, 0, movementArea.position.z);
            latLngPositionLeftTop = VectorExtensions.GetGeoPosition(movementAreaPosition, map.CenterMercator, map.WorldRelativeScale);
            latLngPositionRightBottom = VectorExtensions.GetGeoPosition(movementAreaPosition + new float3(size, 0f, -size), map.CenterMercator, map.WorldRelativeScale);

            // Updating vectors by using the unity positions
            positionLeftTop = leftTop.transform.position;
            positionRightBottom = rightBottom.transform.position;
        }


        /// <summary>
        ///     Sets to a new position and centers around it.
        /// </summary>
        /// <param name="tileLatLngPosition"></param>
        public void SetPosition(Vector2d tileLatLngPosition) {
            
            var size = GetSize();
            var tileSize = TileCoverDebugExtension.GetTileSize(map, 14);

            // Sets the movementarea center to the center of the tile it lies in.
            var movementAreaTransform = movementArea.transform;
            movementAreaTransform.position = new Vector2d(tileLatLngPosition.y, tileLatLngPosition.x).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
            movementAreaTransform.position += new Vector3(tileSize / 2, 0, -(tileSize / 2));
            movementAreaTransform.position += new Vector3(-(size / 2), 0, size / 2);
        }


        /////////////////////////////////
        /// Getter/Setter methods
        /////////////////////////////////


        /// <summary>
        ///     Returns the size of the movement area by substracting the corners from each other.
        /// </summary>
        /// <returns></returns>
        public float GetSize() { return rightBottom.transform.position.x - leftTop.transform.position.x; }


        /// <summary>
        ///     This method checks if a certain lat/lng position lays within the movement area... if so it returns true.
        /// </summary>
        /// <param name="geoLocation">The geolocation we check if it lies within the area</param>
        /// <returns></returns>
        public bool InArea(Vector2d geoLocation) {
            return geoLocation.x <= latLngPositionLeftTop.x && geoLocation.x >= latLngPositionRightBottom.x &&
                   geoLocation.y >= latLngPositionLeftTop.y && geoLocation.y <= latLngPositionRightBottom.y;
        }

        /// <summary>
        ///     Returns true if the transform is within the area bounds.
        /// </summary>
        /// <param name="transform">The transform we wanna check if it lies within the area</param>
        /// <returns></returns>
        public bool InArea(Vector3 transform) {
            return transform.x <= positionRightBottom.x && transform.x >= positionLeftTop.x &&
                   transform.z >= positionRightBottom.z && transform.z <= positionLeftTop.z;
        }
        
        /////////////////////////////////
        /// Debug methods
        /////////////////////////////////
        
             
        /// <summary>
        /// Returns the size of a tile.
        /// </summary>
        /// <param name="z">The zoom level we wanna get the tile size of</param>
        /// <returns>The size of a tile ( unity size ) at the given zoom level</returns>
        public float GetTileSize(int z) {
            
            var latLngPosition = VectorExtensions.GetGeoPosition(gameObject.transform.position, map.CenterMercator,map.WorldRelativeScale);
            
            var tile = TileCover.CoordinateToTileId(latLngPosition, z);
            var newTile = new UnwrappedTileId(z, tile.X+1, tile.Y+1);

            var oldPos = TileCover.TileIdToCoordinate(tile);
            var newPos = TileCover.TileIdToCoordinate(newTile);
            
            var oldPosition = VectorExtensions.AsUnityPosition(new Vector2d(oldPos.y, oldPos.x),  map.CenterMercator, map.WorldRelativeScale);
            var newPosition = VectorExtensions.AsUnityPosition(new Vector2d(newPos.y, newPos.x),  map.CenterMercator, map.WorldRelativeScale);
            
            return newPosition.x - oldPosition.x;
        }
        
        
        /// <summary>
        /// Used for debuging, spawns in several tiles and marks their area.
        /// </summary>
        /// <param name="tile"></param>
        public void DebugTiles(UnwrappedTileId tile, int zoom) {

            var map = ServiceLocator.Get<AbstractMap>();
            
            var startX = tile.X;
            var startY = tile.Y;
            var tileSize = GetTileSize(tile.Z);

            // Cubes makieren zenter der tiles.
            for (var index = startX - 2; index <= startX + 2; index++) {

                for (var sindex = startY-2; sindex <= startY + 2; sindex++) {
                    
                    var newTile = new UnwrappedTileId(zoom, index, sindex);
                    var pos = TileCover.TileIdToCoordinate(newTile);
                    var unityPos = VectorExtensions.AsUnityPosition(new Vector2d(pos.y, pos.x),  map.CenterMercator, map.WorldRelativeScale);
                    
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(unityPos.x+(tileSize/2), unityPos.y, unityPos.z-(tileSize/2));
                    cube.transform.localScale = new Vector3(tileSize-5,1,tileSize-5);
                    cube.name = newTile.X + " " + newTile.Y + " " + newTile.Z;
                }
            }
        }
        
        /// <summary>
        /// Used for debuging, spawns in several tiles and marks their area.
        /// </summary>
        /// <param name="tile"></param>
        public void DebugTiles(UnwrappedTileId tile, int zoom, Color color) {

            var map = ServiceLocator.Get<AbstractMap>();
            
            var startX = tile.X;
            var startY = tile.Y;
            var tileSize = GetTileSize(tile.Z);

            // Cubes makieren zenter der tiles.
            for (var index = startX - 2; index <= startX + 2; index++) {

                for (var sindex = startY-2; sindex <= startY + 2; sindex++) {
                    
                    var newTile = new UnwrappedTileId(zoom, index, sindex);
                    var pos = TileCover.TileIdToCoordinate(newTile);
                    var unityPos = VectorExtensions.AsUnityPosition(new Vector2d(pos.y, pos.x),  map.CenterMercator, map.WorldRelativeScale);
                    
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    var renderer = cube.GetComponent<Renderer>();
                    cube.transform.position = new Vector3(unityPos.x+(tileSize/2), unityPos.y, unityPos.z-(tileSize/2));
                    cube.transform.localScale = new Vector3(tileSize-5,1,tileSize-5);
                    cube.name = newTile.X + " " + newTile.Y + " " + newTile.Z;
                    renderer.material.color = color;
                }
            }
        }
    }
}