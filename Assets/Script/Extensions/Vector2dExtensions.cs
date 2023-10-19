using System;
using Mapbox.Map;
using Mapbox.Unity.Utilities;
using ParallelOrigin.Core.Base.Classes;
using UnityEngine;

namespace Script.Extensions {
    
    /// <summary>
    /// An own vector2d implementation for working with the custom <see cref="Vector2d"/>
    /// </summary>
    public static class Vector2dExtensions {

        private const int TileSize = 256;
        /// <summary>according to https://wiki.openstreetmap.org/wiki/Zoom_levels</summary>
        private const int EarthRadius = 6378137; //no seams with globe example
        private const double InitialResolution = 2 * Math.PI * EarthRadius / TileSize;
        private const double OriginShift = 2 * Math.PI * EarthRadius / 2;

        
        /// <summary>
        /// Vector2d convenience method to convert Vector2d to Vector3.
        /// </summary>
        /// <returns>Vector3 with a y value of zero.</returns>
        /// <param name="v">Vector2d.</param>
        public static Vector3 ToVector3XZ(this Vector2d v) {
            return new Vector3((float)v.X, 0, (float)v.Y);
        }
        
        /// <summary>
        /// Vector2 extension method to convert from a latitude/longitude to a Unity Vector3.
        /// </summary>
        /// <returns>The Vector3 Unity position.</returns>
        /// <param name="latLon">Latitude Longitude.</param>
        /// <param name="refPoint">Reference point.</param>
        /// <param name="scale">Scale.</param>
        public static Vector3 AsUnityPosition(this Vector2d latLon, Vector2d refPoint, float scale = 1) {
            return GeoToWorldPosition(latLon.X, latLon.Y, refPoint, scale).ToVector3XZ();
        }
        
        /// <summary>
        /// Converts WGS84 lat/lon to x/y meters in reference to a center point
        /// </summary>
        /// <param name="lat"> The latitude. </param>
        /// <param name="lon"> The longitude. </param>
        /// <param name="refPoint"> A <see cref="T:UnityEngine.Vector2d"/> center point to offset resultant xy, this is usually map's center mercator</param>
        /// <param name="scale"> Scale in meters. (default scale = 1) </param>
        /// <returns> A <see cref="T:UnityEngine.Vector2d"/> xy tile ID. </returns>
        /// <example>
        /// Converts a Lat/Lon of (37.7749, 122.4194) into Unity coordinates for a map centered at (10,10) and a scale of 2.5 meters for every 1 Unity unit 
        /// <code>
        /// var worldPosition = Conversions.GeoToWorldPosition(37.7749, 122.4194, new Vector2d(10, 10), (float)2.5);
        /// // worldPosition = ( 11369163.38585, 34069138.17805 )
        /// </code>
        /// </example>
        public static Vector2d GeoToWorldPosition(double lat, double lon, Vector2d refPoint, float scale = 1) {
            var posx = lon * OriginShift / 180;
            var posy = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            posy = posy * OriginShift / 180;
            return new Vector2d((posx - refPoint.X) * scale, (posy - refPoint.Y) * scale);
        }
        
        /// <summary> Converts a coordinate to a tile identifier. </summary>
        /// <param name="coord"> Geographic coordinate. </param>
        /// <param name="zoom"> Zoom level. </param>
        /// <returns>The to tile identifier.</returns>
        /// <example>
        /// Convert a geocoordinate to a TileId:
        /// <code>
        /// var unwrappedTileId = TileCover.CoordinateToTileId(new Vector2d(40.015, -105.2705), 18);
        /// Console.Write("UnwrappedTileId: " + unwrappedTileId.ToString());
        /// </code>
        /// </example>
        public static UnwrappedTileId CoordinateToTileId(Vector2d coord, int zoom) {
            
            var lat = coord.X;
            var lng = coord.Y;

            // See: http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
            var x = (int)Math.Floor((lng + 180.0) / 360.0 * Math.Pow(2.0, zoom));
            var y = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0)
                                                    + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));

            return new UnwrappedTileId(zoom, x, y);
        }
    }
}